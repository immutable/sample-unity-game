#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Orderbook.Api;
using Immutable.Orderbook.Client;
using Immutable.Orderbook.Model;
using Immutable.Passport;
using Immutable.Passport.Model;
using Immutable.Search.Api;
using Immutable.Search.Model;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using ApiException = Immutable.Search.Client.ApiException;

namespace HyperCasual.Runner
{
    public class AssetDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;

        [SerializeField] private ImageUrlObject m_Image;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;

        // Market
        [SerializeField] private GameObject m_MarketContainer;
        [SerializeField] private TextMeshProUGUI m_FloorPriceText;

        [SerializeField] private TextMeshProUGUI m_LastTradePriceText;

        // Details
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private TextMeshProUGUI m_CollectionText;
        [SerializeField] private TextMeshProUGUI m_ContractTypeText;

        // Attributes
        [SerializeField] private GameObject m_AttributesContainer;
        [SerializeField] private Transform m_AttributesListParent;
        [SerializeField] private AttributeView m_AttributeObj;

        // Listing
        [SerializeField] private GameObject m_ListingContainer;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private HyperCasualButton m_SellButton;
        [SerializeField] private HyperCasualButton m_CancelButton;
        [SerializeField] private GameObject m_Progress;

        [SerializeField] private CustomDialog m_CustomDialog;

        private readonly List<AttributeView> m_Attributes = new();

        private readonly SearchApi m_SearchApi;
        private readonly OrderbookApi m_TsApi;

        private InventoryScreen.AssetType m_Type;
        private AssetModel m_Asset;
        private OldListing? m_Listing;

        public AssetDetailsView()
        {
            var tsConfig = new Configuration();
            tsConfig.BasePath = Config.BASE_URL;
            m_TsApi = new OrderbookApi(tsConfig);

            var searchConfig = new Immutable.Search.Client.Configuration();
            searchConfig.BasePath = Config.BASE_URL;
            m_SearchApi = new SearchApi(searchConfig);
        }

        private void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Disable the template attribute object

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);
            m_SellButton.RemoveListener(OnSellButtonClicked);
            m_SellButton.AddListener(OnSellButtonClicked);
            m_CancelButton.RemoveListener(OnCancelButtonClicked);
            m_CancelButton.AddListener(OnCancelButtonClicked);

            // Gets the player's balance
            m_Balance.UpdateBalance();
        }

        /// <summary>
        ///     Cleans up data
        /// </summary>
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

        /// <summary>
        ///     Initialises the UI based on the asset.
        /// </summary>
        /// <param name="asset">The asset to display.</param>
        public async void Initialise(InventoryScreen.AssetType assetType, AssetModel asset)
        {
            m_Type = assetType;
            m_Asset = asset;

            m_NameText.text = m_Asset.contract_type switch
            {
                "ERC721" => $"{m_Asset.name} #{m_Asset.token_id}",
                "ERC1155" => $"{m_Asset.name} x{m_Asset.balance}",
                _ => m_NameText.text
            };

            m_DescriptionText.text = m_Asset.description;
            m_DescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(m_Asset.description));

            m_TokenIdText.text = $"Token ID: {m_Asset.token_id}";
            m_CollectionText.text = $"Collection: {m_Asset.contract_address}";
            m_ContractTypeText.text = $"Contract type: {m_Asset.contract_type}";

            // Clear existing attributes
            ClearAttributes();

            // Download and display the image
            m_Image.LoadUrl(m_Asset.image);

            switch (m_Type)
            {
                case InventoryScreen.AssetType.Skin:
                    // Populate attributes
                    foreach (var a in m_Asset.attributes)
                    {
                        NFTMetadataAttribute attribute = new(traitType: a.trait_type,
                            value: new NFTMetadataAttributeValue(a.value));
                        var newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent);
                        newAttribute.gameObject.SetActive(true);
                        newAttribute.Initialise(attribute);
                        m_Attributes.Add(newAttribute);
                    }

                    // Check if asset is listed
                    m_Listing = await GetActiveListingId();
                    m_SellButton.gameObject.SetActive(m_Listing == null);
                    m_CancelButton.gameObject.SetActive(m_Listing != null);

                    // Price if it's listed
                    m_AmountText.text = "-";
                    if (m_Listing != null)
                    {
                        var amount = m_Listing.buy[0].amount;
                        var quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                        m_AmountText.text = $"{quantity} IMR";
                    }
                    else
                    {
                        m_AmountText.text = "Not listed";
                    }

                    m_FloorPriceText.text = "Floor price: -";
                    m_LastTradePriceText.text = "Last trade price: -";
                    GetMarketData();
                    break;
                case InventoryScreen.AssetType.Powerups:
                    break;
            }

            m_ListingContainer.SetActive(m_Type == InventoryScreen.AssetType.Skin);
            m_AttributesContainer.SetActive(m_Type == InventoryScreen.AssetType.Skin);
            m_MarketContainer.SetActive(m_Type == InventoryScreen.AssetType.Skin);
        }

        private async void GetMarketData()
        {
            try
            {
                var response = await m_SearchApi.QuotesForStacksAsync(Config.CHAIN_NAME, m_Asset.contract_address,
                    new List<Guid> { Guid.Parse(m_Asset.metadata_id) });
                if (response.Result.Count > 0)
                {
                    var quote = response.Result[0];
                    var market = quote.MarketStack;

                    if (market?.FloorListing != null)
                    {
                        var quantity = (decimal)BigInteger.Parse(market.FloorListing.PriceDetails.Amount.Value) /
                                       (decimal)BigInteger.Pow(10, 18);
                        m_FloorPriceText.text = $"Floor price: {quantity} IMR";
                    }
                    else
                    {
                        m_FloorPriceText.text = "Floor price: N/A";
                    }

                    if (market?.LastTrade?.PriceDetails?.Count > 0)
                    {
                        var quantity = (decimal)BigInteger.Parse(market.LastTrade.PriceDetails[0].Amount.Value) /
                                       (decimal)BigInteger.Pow(10, 18);
                        m_LastTradePriceText.text = $"Last trade price: {quantity} IMR";
                    }
                    else
                    {
                        m_LastTradePriceText.text = "Last trade price: N/A";
                    }
                }
            }
            catch (ApiException e)
            {
                Debug.LogError("Exception when calling: " + e.Message);
                Debug.LogError("Status Code: " + e.ErrorCode);
                Debug.LogError(e.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get market data: {ex.Message}");
            }
        }

        // TODO not required one we have the NFT search endpoint
        private async UniTask<OldListing?> GetActiveListingId()
        {
            try
            {
                using var client = new HttpClient();
                var url =
                    $"{Config.BASE_URL}/v1/chains/{Config.CHAIN_NAME}/orders/listings?sell_item_contract_address={m_Asset.contract_address}&sell_item_token_id={m_Asset.token_id}&status=ACTIVE";
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

        /// <summary>
        ///     Handles the click event for the sell button.
        /// </summary>
        private async void OnSellButtonClicked()
        {
            var (result, price) = await m_CustomDialog.ShowDialog(
                $"List {m_Asset.name} for sale",
                "Enter your price below (in IMR):",
                "Confirm",
                "Cancel",
                true
            );

            if (result)
            {
                m_SellButton.gameObject.SetActive(false);
                m_Progress.gameObject.SetActive(true);

                var amount = Math.Floor(decimal.Parse(price) * (decimal)BigInteger.Pow(10, 18));

                var listingId = await Sell($"{amount}");
                Debug.Log($"Sell complete: Listing ID: {listingId}");

                m_SellButton.gameObject.SetActive(listingId == null);
                m_CancelButton.gameObject.SetActive(listingId != null);
                m_AmountText.text = listingId != null ? $"{price} IMR" : "Not listed";
                m_Progress.gameObject.SetActive(false);
            }
        }

        /// <summary>
        ///     Gets the details for the listing
        /// </summary>
        private async UniTask<Listing> GetListing(string listingId) // TODO To replace with get stack by ID endpoint
        {
            try
            {
                using var client = new HttpClient();
                var url = $"{Config.BASE_URL}/v1/chains/{Config.CHAIN_NAME}/orders/listings/{listingId}";
                Debug.Log($"Get listing URL: {url}");

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var orderResponse = JsonUtility.FromJson<OrderResponse>(responseBody);

                    return new Listing(
                        orderResponse.result.id,
                        new PriceDetails
                        (
                            new PriceDetailsToken(new ERC20Token(symbol: "IMR", contractAddress: Contract.TOKEN,
                                decimals: 18)),
                            new PaymentAmount(orderResponse.result.buy[0].amount, orderResponse.result.buy[0].amount),
                            new PaymentAmount(orderResponse.result.buy[0].amount,
                                orderResponse.result.buy[0].amount), // Mocked
                            orderResponse.result.fees.Select(fee => new Immutable.Search.Model.Fee(
                                    fee.amount, Immutable.Search.Model.Fee.TypeEnum.ROYALTY, fee.recipient_address))
                                .ToList()
                        ),
                        orderResponse.result.sell[0].token_id,
                        orderResponse.result.account_address,
                        "1"
                    );
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Failed to get listing: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to get listing: {ex.Message}");
            }

            return null;
        }

        private async UniTask<PrepareListing200Response> PrepareListing(
            string nftTokenAddress, string tokenId, string price, string erc20TokenAddress)
        {
            // Define the NFT to sell, using its contract address and token ID
            var nft = new ERC721Item(nftTokenAddress, tokenId);

            // Define the ERC20 token that the buyer will use to purchase the NFT
            var buy = new ERC20Item(price, erc20TokenAddress);

            // Call the Orderbook function to prepare the listing for sale
            return await m_TsApi.PrepareListingAsync(
                new PrepareListingRequest
                (
                    makerAddress: SaveManager.Instance.WalletAddress,
                    sell: new PrepareListingRequestSell(nft),
                    buy: new PrepareListingRequestBuy(buy)
                ));
        }

        private async UniTask SignAndSubmitApproval(PrepareListing200Response prepareListingResponse)
        {
            var transactionAction = prepareListingResponse.Actions.FirstOrDefault(action =>
                ReferenceEquals(action.ActualInstance, typeof(TransactionAction)));
            // Send approval transaction if it is required
            if (transactionAction != null)
            {
                var tx = transactionAction.GetTransactionAction();
                var transactionResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                    new TransactionRequest
                    {
                        to = tx.PopulatedTransactions.To,
                        data = tx.PopulatedTransactions.Data,
                        value = "0"
                    });

                if (transactionResponse.status != "1") throw new Exception("Failed to sign and submit approval");
            }
        }

        private async UniTask<string> SignListing(PrepareListing200Response prepareListingResponse)
        {
            var signableAction =
                prepareListingResponse.Actions.FirstOrDefault(action => action.GetSignableAction() != null);

            if (signableAction == null) throw new Exception("No listing to sign");

            var message = signableAction.GetSignableAction().Message;

            // Use Unity Passport package to sign typed data function to sign the listing payload
            return await Passport.Instance.ZkEvmSignTypedDataV4(
                      JsonConvert.SerializeObject(message, Formatting.Indented));
        }

        /// <summary>
        ///     Prepares the listing for the asset.
        /// </summary>
        /// <param name="price">The price of the asset in smallest unit.</param>
        /// <returns>The listing ID is asset was successfully listed</returns>
        private async UniTask<string?> Sell(string price)
        {
            try
            {
                PrepareListing200Response prepareListingResponse =
                    await PrepareListing(m_Asset.contract_address, m_Asset.token_id, $"{price}", Contract.TOKEN);

                await SignAndSubmitApproval(prepareListingResponse);

                var signature = await SignListing(prepareListingResponse);

                var listingId = await ListAsset(signature, prepareListingResponse);
                Debug.Log($"Listing ID: {listingId}");

                await ConfirmListingStatus(listingId, "ACTIVE");

                return listingId;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell: {ex.Message}");
                Debug.LogError(ex.StackTrace);
                await m_CustomDialog.ShowDialog("Failed to sell", ex.Message, "OK");
            }

            return null;
        }

        /// <summary>
        ///     Finalises the listing of the asset.
        /// </summary>
        /// <param name="signature">The signature for the listing.</param>
        /// <param name="preparedListing">The prepared listing data.</param>
        private async UniTask<string> ListAsset(string signature,
            PrepareListing200Response preparedListing)
        {
            var createListingResponse = await m_TsApi.CreateListingAsync(
                new CreateListingRequest
                (
                    new List<FeeValue>(),
                    preparedListing.OrderComponents,
                    preparedListing.OrderHash,
                    signature
                ));

            return createListingResponse.Result.Id;
        }

        /// <summary>
        ///     Cancels the listing of the asset.
        /// </summary>
        private async void OnCancelButtonClicked()
        {
            Debug.Log($"Cancel listing {m_Listing.id}");

            m_CancelButton.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(true);

            try
            {
                var request = new CancelOrdersOnChainRequest(
                    accountAddress: SaveManager.Instance.WalletAddress,
                    orderIds: new List<string> { m_Listing.id });
                var response = await m_TsApi.CancelOrdersOnChainAsync(request);

                if (response?.CancellationAction.PopulatedTransactions.To != null)
                {
                    var transactionResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                        new TransactionRequest
                        {
                            to = response.CancellationAction.PopulatedTransactions.To, // Immutable seaport contract
                            data = response.CancellationAction.PopulatedTransactions.Data, // fd9f1e10 cancel
                            value = "0"
                        });

                    if (transactionResponse.status == "1")
                    {
                        // Validate that listing has been cancelled
                        await ConfirmListingStatus(m_Listing.id, "CANCELLED");

                        // TODO update to use get stack bundle by stack ID endpoint instead

                        m_SellButton.gameObject.SetActive(true);
                        m_Progress.gameObject.SetActive(false);
                        m_AmountText.text = "Not listed";

                        return;
                    }
                }

                m_Progress.gameObject.SetActive(false);
                m_CancelButton.gameObject.SetActive(true);
                await m_CustomDialog.ShowDialog("Error", "Failed to cancel listing", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                m_Progress.gameObject.SetActive(false);
                m_CancelButton.gameObject.SetActive(true);
                await m_CustomDialog.ShowDialog("Error", "Failed to cancel listing", "OK");
            }
        }

        /// <summary>
        ///     Polls the listing status until it transitions to the given status or the operation times out after 1 minute.
        /// </summary>
        private async UniTask ConfirmListingStatus(string listingId, string status)
        {
            Debug.Log($"Confirming listing {listingId} is {status}...");

            var conditionMet = await PollingHelper.PollAsync(
                $"{Config.BASE_URL}/v1/chains/imtbl-zkevm-devnet/orders/listings/{listingId}",
                responseBody =>
                {
                    var listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);
                    m_Listing = listingResponse.result;
                    return listingResponse.result?.status.name == status;
                });

            if (conditionMet)
                await m_CustomDialog.ShowDialog("Success", $"Listing is {status.ToLower()}.", "OK");
            else
                await m_CustomDialog.ShowDialog("Error", $"Failed to confirm if listing is {status.ToLower()}.", "OK");
        }

        private void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }

        /// <summary>
        ///     Removes all the attribute views
        /// </summary>
        private void ClearAttributes()
        {
            foreach (var attribute in m_Attributes) Destroy(attribute.gameObject);
            m_Attributes.Clear();
        }
    }
}