using System;
using System.Net.Http;
using System.Numerics;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents an asset in the player's inventory
    /// </summary>
    public class AssetListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private ImageUrlObject m_Image;

        private AssetModel m_Asset;

        /// <summary>
        ///     Sets up the inventory list and fetches the player's assets.
        /// </summary>
        private void OnEnable()
        {
            UpdateData();
        }

        /// <summary>
        ///     Initialises the asset object with relevant data and updates the UI.
        /// </summary>
        public void Initialise(AssetModel asset)
        {
            m_Asset = asset;
            UpdateData();
        }

        /// <summary>
        ///     Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            if (m_Asset != null)
            {
                m_NameText.text = $"{m_Asset.name} #{m_Asset.token_id}";
                m_AmountText.text = "-";
                m_Image.LoadUrl(m_Asset.image);

                OldListing listing = await GetActiveListingId();
                if (listing != null)
                {
                    var amount = listing.buy[0].amount;
                    var quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                    m_AmountText.text = $"{quantity} IMR";
                }
                else
                {
                    m_AmountText.text = "Not listed";
                }
            }
        }
        
        // TODO to remove
        private async UniTask<OldListing> GetActiveListingId()
        {
            try
            {
                using var client = new HttpClient();
                var url =
                    $"{Config.BASE_URL}/v1/chains/{Config.CHAIN_NAME}/orders/listings?sell_item_contract_address={Contract.SKIN}&sell_item_token_id={m_Asset.token_id}&status=ACTIVE";
                Debug.Log($"GetActiveListingId URL: {url}");

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var listingResponse = JsonUtility.FromJson<ListingsResponse>(responseBody);

                    // Check if the listing exists
                    if (listingResponse.result.Count > 0 && listingResponse.result[0].status.name == "ACTIVE")
                        return listingResponse.result[0];
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to check sale status: {ex.Message}");
            }

            return null;
        }
    }
}