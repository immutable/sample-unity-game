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
            try
            {
                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/collections/{Contract.SKIN}/nfts/{tokenId}";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    AssetResponse assetResponse = JsonUtility.FromJson<AssetResponse>(responseBody);
                    if (assetResponse?.result != null)
                    {
                        m_NameText.text = assetResponse.result.name;
                        if (!string.IsNullOrEmpty(assetResponse.result.image))
                        {
                            await DownloadImage(assetResponse.result.image);
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
        private async UniTask DownloadImage(string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                await request.SendWebRequest();

                if (m_Image == null)
                {
                    return;
                }

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
}
