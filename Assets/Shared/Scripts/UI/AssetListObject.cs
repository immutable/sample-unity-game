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
        [SerializeField] private ImageUrlObject m_Image = null;

        private AssetModel m_Asset;

        /// <summary>
        /// Initialises the asset object with relevant data and updates the UI.
        /// </summary>
        public void Initialise(AssetModel asset)
        {
            m_Asset = asset;
            UpdateData();
        }

        /// <summary>
        /// Sets up the inventory list and fetches the player's assets.
        /// </summary>
        private void OnEnable()
        {
            Debug.Log("AssetListObject OnEnable");
            UpdateData();
        }

        /// <summary>
        /// Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            if (m_Asset != null)
            {
                m_NameText.text = m_Asset.name;
                m_CollectionText.text = m_Asset.contract_address;

                // Fetch sale status
                bool isOnSale = await IsListed(m_Asset.token_id);
                m_StatusText.text = isOnSale ? "Listed" : "Not listed";

                m_Image.LoadUrl(m_Asset.image);
            }
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
                    ListingsResponse listingsResponse = JsonUtility.FromJson<ListingsResponse>(responseBody);

                    // Check if the listing exists
                    return listingsResponse.result.Length > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to check sale status: {ex.Message}");
            }

            return false;
        }
    }
}