#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Api.ZkEvm.Api;
using Immutable.Api.ZkEvm.Client;
using Immutable.Api.ZkEvm.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class InventoryAssetDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private ImageUrlObject m_Image;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private TextMeshProUGUI m_FloorPriceText;
        [SerializeField] private TextMeshProUGUI m_LastTradePriceText;
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private TextMeshProUGUI m_CollectionText;
        [SerializeField] private TextMeshProUGUI m_ContractTypeText;
        [SerializeField] private Transform m_AttributesListParent;
        [SerializeField] private AttributeView m_AttributeObj;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private HyperCasualButton m_SellButton;
        [SerializeField] private HyperCasualButton m_CancelButton;
        [SerializeField] private GameObject m_Progress;
        [SerializeField] private CustomDialog m_CustomDialog;

        private MetadataSearchApi m_MetadataSearchApi = new(new Configuration { BasePath = Config.BASE_URL });
        private readonly List<AttributeView> m_Attributes = new();
        private NFTBundle? m_Asset;
        private string? m_ListingId;

        private void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Disable template attribute object

            SetupButtonListeners();
            m_Balance.UpdateBalance(); // Get player balance
        }

        /// <summary>
        /// Setups up listeners for UI buttons.
        /// </summary>
        private void SetupButtonListeners()
        {
            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            m_SellButton.RemoveListener(OnSellButtonClicked);
            m_SellButton.AddListener(OnSellButtonClicked);

            m_CancelButton.RemoveListener(OnCancelButtonClicked);
            m_CancelButton.AddListener(OnCancelButtonClicked);
        }

        /// <summary>
        /// Initialises the asset details view based on the provided asset.
        /// </summary>
        /// <param name="asset">The NFT asset to display.</param>
        public async void Initialise(NFTBundle asset)
        {
            m_Asset = asset;

            var nft = asset.NftWithStack;

            // Name
            m_NameText.text = nft.ContractType switch
            {
                MarketplaceContractType.ERC721 => $"{nft.Name} #{nft.TokenId}",
                MarketplaceContractType.ERC1155 => $"{nft.Name} x{nft.Balance}",
                _ => nft.Name
            };

            // Description
            m_DescriptionText.text = nft.Description;
            m_DescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(nft.Description));

            // Details
            m_TokenIdText.text = $"Token ID: {nft.TokenId}";
            m_CollectionText.text = $"Collection: {nft.ContractAddress}";
            m_ContractTypeText.text = $"Contract type: {nft.ContractType.ToString()}";

            // Attributes
            ClearAttributes();
            var attributes = asset.NftWithStack.Attributes ?? new List<NFTMetadataAttribute>();
            foreach (var attribute in attributes)
            {
                var attributeView = Instantiate(m_AttributeObj, m_AttributesListParent);
                attributeView.gameObject.SetActive(true);
                attributeView.Initialise(attribute);
                m_Attributes.Add(attributeView);
            }

            UpdateListingStateAndPrice();

            // Market data
            var floorListing = m_Asset!.Market?.FloorListing;
            m_FloorPriceText.text = floorListing != null
                ? $"Floor price: {GetQuantity(floorListing.PriceDetails.Amount)} IMR"
                : "Floor price: N/A";

            var lastTrade = m_Asset.Market?.LastTrade?.PriceDetails?[0];
            m_LastTradePriceText.text = lastTrade != null
                ? $"Last trade price: {GetQuantity(lastTrade.Amount)} IMR"
                : "Last trade price: N/A";

            await LoadAssetImage();
        }

        /// <summary>
        /// Updates the listing state and sets the listing price based on the asset's listing data.
        /// </summary>
        private void UpdateListingStateAndPrice()
        {
            // Determine the listing ID based on the asset's listings
            m_ListingId = m_Asset!.Listings.Count > 0 ? m_Asset.Listings[0].ListingId : null;

            // Set the visibility of the sell and cancel buttons
            m_SellButton.gameObject.SetActive(m_ListingId == null);
            m_CancelButton.gameObject.SetActive(m_ListingId != null);

            // Update the listing price text
            if (m_ListingId == null)
            {
                m_AmountText.text = "Not listed";
                return;
            }

            var listing = m_Asset.Listings[0];
            var quantity = GetQuantity(listing.PriceDetails.Amount);

            // Set the amount text based on the contract type
            m_AmountText.text = m_Asset.NftWithStack.ContractType switch
            {
                MarketplaceContractType.ERC721 => $"{quantity} IMR",
                MarketplaceContractType.ERC1155 => $"{listing.Amount} for {quantity} IMR",
                _ => m_AmountText.text
            };
        }

        /// <summary>
        /// Converts a raw token value to decimal.
        /// </summary>
        private static decimal GetQuantity(string value) =>
            (decimal)BigInteger.Parse(value) / (decimal)BigInteger.Pow(10, 18);

        /// <summary>
        /// Loads the asset image asynchronously.
        /// </summary>
        private async UniTask LoadAssetImage() =>
            await m_Image.LoadUrl(m_Asset!.NftWithStack.Image);

        /// <summary>
        /// Fetches the latest NFT bundle data.
        /// </summary>
        private async UniTask<NFTBundle?> GetNftBundle()
        {
            if (m_Asset == null) return null;

            try
            {
                var result = await m_MetadataSearchApi.SearchNFTsAsync(
                    chainName: Config.CHAIN_NAME,
                    contractAddress: new List<string> { m_Asset.NftWithStack.ContractAddress },
                    accountAddress: SaveManager.Instance.WalletAddress,
                    stackId: new List<Guid> { m_Asset.NftWithStack.StackId },
                    onlyIncludeOwnerListings: true);

                if (result.Result.Count > 0)
                {
                    return result.Result
                        .Where(n => n.NftWithStack.TokenId == m_Asset.NftWithStack.TokenId)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to fetch NFT bundle data: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Handles the sell button click event.
        /// </summary>
        private async void OnSellButtonClicked()
        {
            if (m_Asset == null) return;

            var (confirmedPrice, price) = await m_CustomDialog.ShowDialog(
                $"List {m_Asset.NftWithStack.Name} for sale",
                "Enter your price below (in IMR):",
                "Confirm",
                "Cancel",
                true
            );

            if (!confirmedPrice) return;

            var normalisedPrice = Math.Floor(decimal.Parse(price) * (decimal)BigInteger.Pow(10, 18));
            var amountToSell = "1";

            if (m_Asset.NftWithStack.ContractType == MarketplaceContractType.ERC1155)
            {
                var (confirmedAmount, amount) = await m_CustomDialog.ShowDialog(
                    $"How many {m_Asset.NftWithStack.Name} would you like to sell?",
                    "Enter the amount below:",
                    "Confirm",
                    "Cancel",
                    true
                );

                if (!confirmedAmount) return;
                amountToSell = amount;
            }

            m_SellButton.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(true);

            try
            {
                m_ListingId = await CreateOrderUseCase.Instance.CreateListing(
                    contractAddress: m_Asset.NftWithStack.ContractAddress,
                    contractType: m_Asset.NftWithStack.ContractType.ToString(),
                    tokenId: m_Asset.NftWithStack.TokenId,
                    price: $"{normalisedPrice}",
                    amountToSell: amountToSell,
                    buyContractAddress: Contract.TOKEN
                );

                Debug.Log($"Sale complete: Listing ID: {m_ListingId}");

                m_SellButton.gameObject.SetActive(m_ListingId == null);
                m_CancelButton.gameObject.SetActive(m_ListingId != null);

                m_AmountText.text = m_ListingId == null ? "Not listed" :
                    m_Asset!.NftWithStack.ContractType switch
                    {
                        MarketplaceContractType.ERC721 => $"{price} IMR",
                        MarketplaceContractType.ERC1155 => $"{amountToSell} for {price} IMR",
                        _ => m_AmountText.text
                    };

                m_Progress.gameObject.SetActive(false);

                // Fetches the latest NFT bundle data to update the listings.
                var nftBundle = await GetNftBundle();
                if (nftBundle != null)
                {
                    m_Asset.Listings = new List<Listing> { nftBundle.Listings[0] };
                }
            }
            catch (Exception ex)
            {
                m_SellButton.gameObject.SetActive(true);
                await HandleError(ex, "Failed to sell");
            }
        }

        /// <summary>
        ///     Handles the click event of the cancel button.
        /// </summary>
        private async void OnCancelButtonClicked()
        {
            if (m_ListingId == null || m_Asset == null) return;

            m_CancelButton.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(true);

            try
            {
                await CancelListingUseCase.Instance.CancelListing(m_ListingId);

                m_SellButton.gameObject.SetActive(true);
                m_AmountText.text = "Not listed";

                // Remove listing and reset listing ID
                m_Asset.Listings.RemoveAll(listing => listing.ListingId == m_ListingId);
                m_ListingId = null;

                m_Progress.gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                m_CancelButton.gameObject.SetActive(true);
                await HandleError(ex, "Failed to cancel listing");
            }
        }

        private async UniTask HandleError(Exception ex, string errorMessage)
        {
            Debug.LogException(ex);
            m_Progress.gameObject.SetActive(false);
            await m_CustomDialog.ShowDialog("Error", $"{errorMessage}: {ex.Message}", "OK");
        }

        private void OnBackButtonClick() => UIManager.Instance.GoBack();

        private void ClearAttributes()
        {
            foreach (var attribute in m_Attributes) Destroy(attribute.gameObject);
            m_Attributes.Clear();
        }

        private void OnDisable()
        {
            m_NameText.text = "";
            m_DescriptionText.text = "";
            m_TokenIdText.text = "";
            m_CollectionText.text = "";
            m_ContractTypeText.text = "";
            m_AmountText.text = "";
            m_FloorPriceText.text = "";
            m_LastTradePriceText.text = "";
            m_Asset = null;
            ClearAttributes();
        }
    }
}
