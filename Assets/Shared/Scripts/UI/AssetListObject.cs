using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Represents an asset in the player's inventory
    /// </summary>
    public class AssetListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText = null;
        [SerializeField] private TextMeshProUGUI m_TokenIdText = null;
        [SerializeField] private TextMeshProUGUI m_CollectionText = null;
        [SerializeField] private TextMeshProUGUI m_StatusText = null;
        [SerializeField] private RawImage m_Image = null;

        private AssetModel m_Asset;

        /// <summary>
        /// Initialises the asset object with relevant data and updates the UI.
        /// </summary>
        public async void Initialise(AssetModel asset)
        {
            m_Asset = asset;
            UpdateData();

            // Fetch sale status
            bool isOnSale = await IsListed(m_Asset.token_id);
            m_StatusText.text = isOnSale ? "Listed" : "Not listed";

            // Download and display the image
            if (!string.IsNullOrEmpty(m_Asset.image))
            {
                await DownloadImage(m_Asset.image);
            }
        }

        /// <summary>
        /// Updates the text fields with asset data.
        /// </summary>
        private void UpdateData()
        {
            m_NameText.text = m_Asset.name;
            m_CollectionText.text = m_Asset.contract_address;
        }

        /// <summary>
        /// Checks if the asset is listed for sale.
        /// </summary>
        /// <param name="tokenId">The token ID of the asset.</param>
        /// <returns>True if the asset is listed, otherwise false.</returns>
        private async UniTask<bool> IsListed(string tokenId)
        {
            try
            {
                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings?sell_item_contract_address={Contract.SKIN}&sell_item_token_id={tokenId}&status=ACTIVE";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ListingResponse listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);

                    // Check if the listing exists
                    return listingResponse.result.Length > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to check sale status: {ex.Message}");
            }

            return false;
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