using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Passport;
using Immutable.Passport.Model;
using Immutable.Search.Api;
using Immutable.Search.Model;
using Immutable.Ts.Api;
using Immutable.Ts.Client;
using Immutable.Ts.Model;
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

        [SerializeField] private TextMeshProUGUI m_AmountText;

        // Market
        [SerializeField] private TextMeshProUGUI m_FloorPriceText;

        [SerializeField] private TextMeshProUGUI m_LastTradePriceText;

        // Details
        [SerializeField] private TextMeshProUGUI m_TokenIdText;

        [SerializeField] private TextMeshProUGUI m_CollectionText;

        // Attributes
        [SerializeField] private Transform m_AttributesListParent;

        [SerializeField] private AttributeView m_AttributeObj;

        // Actions
        [SerializeField] private HyperCasualButton m_SellButton;
        [SerializeField] private HyperCasualButton m_CancelButton;
        [SerializeField] private GameObject m_Progress;

        // Not listed
        [SerializeField] private GameObject m_EmptyNotListed;
        [SerializeField] private Transform m_NotListedParent;
        [SerializeField] private AssetNotListedObject m_NotListedObj;

        // Listings
        [SerializeField] private GameObject m_EmptyListing;
        [SerializeField] private Transform m_ListingParent;
        [SerializeField] private AssetListingObject m_ListingObj;

        [SerializeField] private CustomDialog m_CustomDialog;

        private readonly List<AttributeView> m_Attributes = new();

        private readonly SearchApi m_SearchApi;
        private readonly DefaultApi m_TsApi;
        private AssetModel m_Asset;
        private OldListing m_Listing;

        public AssetDetailsView()
        {
            var tsConfig = new Configuration();
            tsConfig.BasePath = Config.TS_BASE_URL;
            m_TsApi = new DefaultApi(tsConfig);

            var searchConfig = new Immutable.Search.Client.Configuration();
            searchConfig.BasePath = Config.SEARCH_BASE_URL;
            m_SearchApi = new SearchApi(searchConfig);
        }

        private void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Disable the template attribute object
            m_NotListedObj.gameObject.SetActive(false); // Hide not listed template object
            m_ListingObj.gameObject.SetActive(false); // Hide listing template object

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
            m_TokenIdText.text = "";
            m_CollectionText.text = "";
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
        public async void Initialise(AssetModel asset)
        {
            m_Asset = asset;

            m_NameText.text = m_Asset.name;
            m_TokenIdText.text = $"Token ID: {m_Asset.token_id}";
            m_CollectionText.text = $"Collection: {m_Asset.contract_address}";
            m_AmountText.text = "-";
            m_FloorPriceText.text = "Floor price: -";
            m_LastTradePriceText.text = "Last trade price: -";

            // Clear existing attributes
            ClearAttributes();

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

            // Download and display the image
            m_Image.LoadUrl(m_Asset.image);

            // Check if asset is listed
            m_Listing = await GetActiveListingId();
            m_SellButton.gameObject.SetActive(m_Listing == null);
            m_CancelButton.gameObject.SetActive(m_Listing != null);

            // Price if it's listed
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

            // Get market data
            GetMarketData();
        }

        private async void GetMarketData()
        {
            try
            {
                var response = await m_SearchApi.QuotesForStacksAsync(Config.CHAIN_NAME, Contract.SKIN,
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

        /// <summary>
        ///     Handles the click event for the sell button.
        /// </summary>
        private async void OnSellButtonClicked()
        {
            m_SellButton.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(true);

            var (result, price) = await m_CustomDialog.ShowDialog(
                $"List {m_Asset.name} for sale",
                "Enter your price below (in IMR):",
                "Confirm",
                "Cancel",
                true
            );

            if (result)
            {
                var amount = Math.Floor(decimal.Parse(price) * (decimal)BigInteger.Pow(10, 18));
                var listingId = await PrepareListing($"{amount}");

                m_SellButton.gameObject.SetActive(listingId == null);
                m_CancelButton.gameObject.SetActive(listingId != null);
                m_Progress.gameObject.SetActive(false);

                if (listingId != null)
                {
                    // TODO update to use get stack bundle by stack ID endpoint instead
                    m_AmountText.text = $"{price} IMR";

                    return; // true;
                }
            }

            m_SellButton.gameObject.SetActive(true);
            m_Progress.gameObject.SetActive(false);
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

        /// <summary>
        ///     Prepares the listing for the asset.
        /// </summary>
        /// <param name="price">The price of the asset in smallest unit.</param>
        /// <returns>The listing ID is asset was successfully listed</returns>
        private async UniTask<string> PrepareListing(string price)
        {
            var address = SaveManager.Instance.WalletAddress;

            var data = new PrepareListingRequest
            {
                makerAddress = address,
                sell = new PrepareListingERC721Item
                {
                    contractAddress = Contract.SKIN,
                    tokenId = m_Asset.token_id
                },
                buy = new PrepareListingERC20Item
                {
                    amount = price,
                    contractAddress = Contract.TOKEN
                }
            };

            try
            {
                var prepareListingResponse = await m_TsApi.V1TsSdkOrderbookPrepareListingPostAsync(
                    new V1TsSdkOrderbookPrepareListingPostRequest
                    (
                        makerAddress: address,
                        sell: new V1TsSdkOrderbookPrepareListingPostRequestSell(
                            new ERC721Item(Contract.SKIN, m_Asset.token_id)),
                        buy: new V1TsSdkOrderbookPrepareListingPostRequestBuy(
                            new ERC20Item(price, Contract.TOKEN))
                    ));

                var transactionAction =
                    prepareListingResponse.Actions.FirstOrDefault(action =>
                        action.ActualInstance == typeof(TransactionAction));
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

                    if (transactionResponse.status != "1")
                    {
                        await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing.", "OK");
                        return null;
                    }
                }

                // Sign payload
                var signableAction =
                    prepareListingResponse.Actions.FirstOrDefault(action => action.GetSignableAction() != null);

                if (signableAction != null)
                {
                    var message = signableAction.GetSignableAction().Message;
                    var signature =
                        await Passport.Instance.ZkEvmSignTypedDataV4(
                            JsonConvert.SerializeObject(message, Formatting.Indented));

                    Debug.Log($"Signature: {signature}");

                    // (bool result, string signature) = await m_CustomDialog.ShowDialog(
                    //     "Confirm listing",
                    //     "Enter signed payload:",
                    //     "Confirm",
                    //     negativeButtonText: "Cancel",
                    //     showInputField: true
                    // );
                    // if (result)
                    // {
                    return await ListAsset(signature, prepareListingResponse);
                    // }
                }

                Debug.Log("Failed to sell as there is nothing to sign");
                await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing", "OK");
                return null;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell: {ex.Message}");
                Debug.LogError(ex.StackTrace);
                await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing", "OK");
            }

            return null;
        }

        /// <summary>
        ///     Finalises the listing of the asset.
        /// </summary>
        /// <param name="signature">The signature for the listing.</param>
        /// <param name="preparedListing">The prepared listing data.</param>
        private async UniTask<string?> ListAsset(string signature,
            V1TsSdkOrderbookPrepareListingPost200Response preparedListing)
        {
            try
            {
                var createListingResponse = await m_TsApi.V1TsSdkOrderbookCreateListingPostAsync(
                    new V1TsSdkOrderbookCreateListingPostRequest
                    (
                        new List<FeeValue>(),
                        preparedListing.OrderComponents,
                        preparedListing.OrderHash,
                        signature
                    ));

                Debug.Log($"Listing ID: {createListingResponse.Result.Id}");
                await ConfirmListingStatus(createListingResponse.Result.Id, "ACTIVE");

                return createListingResponse.Result.Id;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to list: {ex.Message}");
                await m_CustomDialog.ShowDialog("Error", "Failed to list", "OK");
                return null;
            }
        }

        /// <summary>
        ///     Cancels the listing of the asset.
        /// </summary>
        private async void OnCancelButtonClicked()
        {
            Debug.Log($"Cancel listing {m_Listing.id}");

            m_CancelButton.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(true);

            var address = SaveManager.Instance.WalletAddress;
            var data = new CancelListingRequest
            {
                accountAddress = address,
                orderIds = new List<string> { m_Listing.id }
            };

            try
            {
                var request = new V1TsSdkOrderbookCancelOrdersOnChainPostRequest(
                    accountAddress: address,
                    orderIds: new List<string> { m_Listing.id });
                var response = await m_TsApi.V1TsSdkOrderbookCancelOrdersOnChainPostAsync(request);

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
                $"https://api.dev.immutable.com/v1/chains/imtbl-zkevm-devnet/orders/listings/{listingId}",
                responseBody =>
                {
                    var listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);
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