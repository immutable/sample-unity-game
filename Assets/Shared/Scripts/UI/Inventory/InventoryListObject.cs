using System.Numerics;
using Immutable.Api.ZkEvm.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    /// Represents an inventory list item in the Inventory screen.
    /// </summary>
    public class InventoryListObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private ImageUrlObject m_Image;

        private NFTBundle m_NFT;

        private void OnEnable()
        {
            UpdateUI();
        }

        /// <summary>
        /// Initialises the object with the given NFT data and updates the UI.
        /// </summary>
        /// <param name="asset">The NFT bundle to display in the inventory.</param>
        public void Initialise(NFTBundle asset)
        {
            m_NFT = asset;
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI fields with relevant NFT data.
        /// </summary>
        private void UpdateUI()
        {
            if (m_NFT == null) return;

            // Display the asset name based on its contract type.
            m_NameText.text = m_NFT?.NftWithStack.ContractType switch
            {
                MarketplaceContractType.ERC721 => $"{m_NFT.NftWithStack.Name} #{m_NFT.NftWithStack.TokenId}",
                MarketplaceContractType.ERC1155 => $"{m_NFT.NftWithStack.Name} x{m_NFT.NftWithStack.Balance}",
                _ => m_NameText.text
            };

            // Display the price if listed, otherwise show "Not listed".
            if (m_NFT.Listings.Count > 0)
            {
                var listing = m_NFT.Listings[0];
                var rawAmount = listing.PriceDetails.Amount;
                var quantity = (decimal)BigInteger.Parse(rawAmount) / (decimal)BigInteger.Pow(10, 18);

                m_AmountText.text = m_NFT.NftWithStack.ContractType switch
                {
                    MarketplaceContractType.ERC721 => $"{quantity} IMR",
                    MarketplaceContractType.ERC1155 => $"{listing.Amount} for {quantity} IMR",
                    _ => m_AmountText.text
                };
            }
            else
            {
                m_AmountText.text = "Not listed";
            }

            // Asynchronously load the asset's image.
#pragma warning disable CS4014
            m_Image.LoadUrl(m_NFT.NftWithStack.Image);
#pragma warning restore CS4014
        }

        private void OnDisable()
        {
            m_NameText.text = "";
            m_AmountText.text = "";
        }
    }
}