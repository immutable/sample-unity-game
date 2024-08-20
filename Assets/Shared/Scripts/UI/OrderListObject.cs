using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Immutable.Passport.Model;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Represents an order list item in the marketplace.
    /// </summary>
    public class OrderListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private RawImage m_Image;

        private OrderModel m_Order;

        /// <summary>
        /// Initialises the order list item with the given order data.
        /// </summary>
        /// <param name="order">The order data to display.</param>
        public async void Initialise(OrderModel order)
        {
            m_Order = order;
            UpdateData();

            // Check if the order belongs to the current user
            string address = await GetWalletAddress();
            bool isUsersListing = order.account_address == address;

            // Get and display asset details
            await GetDetails(order.sell[0].token_id);
        }

        /// <summary>
        /// Updates the UI elements with the order data.
        /// </summary>
        private void UpdateData()
        {
            if (m_Order.buy.Length > 0)
            {
                string amount = m_Order.buy[0].amount;
                decimal quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                m_AmountText.text = $"{quantity} IMR";
            }
        }

        /// <summary>
        /// Fetches and displays asset details for the given token ID.
        /// </summary>
        /// <param name="tokenId">The token ID of the asset.</param>
        private async UniTask GetDetails(string tokenId)
        {
            string skinContractAddress = "0x52A1016eCca06bDBbdd9440E7AA9166bD5366aE1";

            try
            {
                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/collections/{skinContractAddress}/nfts/{tokenId}";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseBody);
                    if (tokenResponse?.result != null)
                    {
                        m_NameText.text = tokenResponse.result.name;
                        if (!string.IsNullOrEmpty(tokenResponse.result.image))
                        {
                            StartCoroutine(DownloadImage(tokenResponse.result.image));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get details: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the player's wallet address.
        /// </summary>
        private async UniTask<string> GetWalletAddress()
        {
            List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
            return accounts.Count > 0 ? accounts[0] : string.Empty; // Return the first wallet address
        }

        /// <summary>
        /// Downloads and displays the image from the given URL.
        /// </summary>
        private IEnumerator DownloadImage(string url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

            // Wait for the web request to complete
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                m_Image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                m_Image.gameObject.SetActive(true);
            }
            else
            {
                m_Image.gameObject.SetActive(false);
            }
        }
    }
}
