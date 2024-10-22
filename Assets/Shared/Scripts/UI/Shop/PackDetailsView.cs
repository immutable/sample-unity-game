#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Text;
using HyperCasual.Core;
using Immutable.Passport;
using Immutable.Passport.Model;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class PackDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private ImageUrlObject m_Image;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private TextMeshProUGUI m_AmountText;

        // Attributes
        [SerializeField] private Transform m_ItemsListParent;

        [SerializeField] private PackItemView m_ItemObj;

        // Actions
        [SerializeField] private HyperCasualButton m_BuyButton;
        [SerializeField] private GameObject m_Progress;

        [SerializeField] private CustomDialog m_CustomDialog;

        private readonly List<PackItemView> m_Items = new();

        private Pack? m_Pack = null;

        private async void OnEnable()
        {
            m_ItemObj.gameObject.SetActive(false); // Disable the templateitem object

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);
            m_BuyButton.RemoveListener(OnBuyButtonClicked);
            m_BuyButton.AddListener(OnBuyButtonClicked);

#pragma warning disable CS4014
            m_Balance.UpdateBalance();
#pragma warning restore CS4014
        }

        /// <summary>
        ///     Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_NameText.text = "";
            m_DescriptionText.text = "";
            m_AmountText.text = "";

            m_Pack = null;
            ClearItems();
        }

        public async void Initialise(Pack pack)
        {
            m_Pack = pack;
            m_NameText.text = pack.name;
            m_DescriptionText.text = pack.description;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            m_Image.LoadUrl(pack.image);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var quantity = (decimal)BigInteger.Parse(pack.price) / (decimal)BigInteger.Pow(10, 18);
            m_AmountText.text = $"{quantity} IMR";

            // Clear existing items
            ClearItems();

            // Populate items
            foreach (var item in m_Pack.items)
            {
                var newItem = Instantiate(m_ItemObj, m_ItemsListParent);
                newItem.gameObject.SetActive(true);
                newItem.Initialise(item);
                m_Items.Add(newItem);
            }

            Debug.Log($"{m_Pack.name}: {m_Pack.function} {m_Pack.collection}");
        }

        private async void OnBuyButtonClicked()
        {
            m_Progress.gameObject.SetActive(true);
            m_BuyButton.gameObject.SetActive(false);

            try
            {
                var requestBody = new
                {
                    amount = m_Pack?.price,
                    address = SaveManager.Instance.WalletAddress
                };
                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                var response = await client.PostAsync($"{Config.SERVER_URL}/pack/checkApprovalRequired", content);

                if (!response.IsSuccessStatusCode)
                {
                    m_Progress.gameObject.SetActive(false);
                    m_BuyButton.gameObject.SetActive(true);
                    await m_CustomDialog.ShowDialog("Failed to buy", "Could not check if approval is required", "OK");
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var approvalTransaction = JsonUtility.FromJson<Transaction>(responseBody);

                if (approvalTransaction.data != null)
                {
                    var approvalResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                        new TransactionRequest
                        {
                            to = approvalTransaction.to,
                            data = approvalTransaction.data,
                            value = approvalTransaction.amount
                        });

                    if (approvalResponse.status != "1")
                    {
                        m_Progress.gameObject.SetActive(false);
                        m_BuyButton.gameObject.SetActive(true);
                        await m_CustomDialog.ShowDialog("Failed to buy", "Could not get approval", "OK");
                        return;
                    }
                }

                var transactionResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                    new TransactionRequest
                    {
                        to = m_Pack.collection,
                        data = m_Pack.function,
                        value = "0"
                    });

                if (transactionResponse.status != "1")
                {
                    m_Progress.gameObject.SetActive(false);
                    m_BuyButton.gameObject.SetActive(true);
                    await m_CustomDialog.ShowDialog("Failed to buy", "Could not buy pack", "OK");
                    return;
                }

                m_Balance.UpdateBalance();
                await m_CustomDialog.ShowDialog("Success", "You bought a pack!", "OK");
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to buy: {ex.Message}");
                Debug.LogError(ex.StackTrace);
                await m_CustomDialog.ShowDialog("Failed to buy", ex.Message, "OK");
            }

            m_BuyButton.gameObject.SetActive(true);
            m_Progress.gameObject.SetActive(false);
        }

        private void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }

        /// <summary>
        ///     Removes all the item views
        /// </summary>
        private void ClearItems()
        {
            foreach (var attribute in m_Items) Destroy(attribute.gameObject);
            m_Items.Clear();
        }
    }
}