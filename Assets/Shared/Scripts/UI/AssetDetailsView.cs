using System;
using System.Collections.Generic;
using System.Numerics;
using System.Net.Http;
using System.Text;
using System.Linq;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Immutable.Passport;
using Immutable.Passport.Model;
using Immutable.Search.Model;

namespace HyperCasual.Runner
{
    public class AssetDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private ImageUrlObject m_Image;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_CollectionText;
        // Attributes
        [SerializeField] private Transform m_AttributesListParent;
        [SerializeField] private AttributeView m_AttributeObj;
        // Not listed
        [SerializeField] private GameObject m_EmptyNotListed;
        [SerializeField] private Transform m_NotListedParent = null;
        private List<AssetNotListedObject> m_NotListedViews = new List<AssetNotListedObject>();
        [SerializeField] private AssetNotListedObject m_NotListedObj = null;

        // Listings
        [SerializeField] private GameObject m_EmptyListing;
        [SerializeField] private Transform m_ListingParent = null;
        private List<AssetListingObject> m_ListingViews = new List<AssetListingObject>();
        [SerializeField] private AssetListingObject m_ListingObj = null;

        [SerializeField] private CustomDialog m_CustomDialog;

        private List<AttributeView> m_Attributes = new List<AttributeView>();
        private StackBundle m_Asset;

        private void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Disable the template attribute object
            m_NotListedObj.gameObject.SetActive(false); // Hide not listed template object
            m_ListingObj.gameObject.SetActive(false); // Hide listing template object

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            // Gets the player's balance
            m_Balance.UpdateBalance();
        }

        /// <summary>
        /// Initialises the UI based on the asset.
        /// </summary>
        /// <param name="asset">The asset to display.</param>
        public async void Initialise(StackBundle asset)
        {
            m_Asset = asset;

            m_NameText.text = m_Asset.Stack.Name;
            m_CollectionText.text = $"Collection: {m_Asset.Stack.ContractAddress}";

            // Clear existing attributes
            ClearAttributes();

            // Populate attributes
            foreach (NFTMetadataAttribute attribute in m_Asset.Stack.Attributes)
            {
                AttributeView newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent);
                newAttribute.gameObject.SetActive(true);
                newAttribute.Initialise(attribute);
                m_Attributes.Add(newAttribute);
            }

            // Download and display the image
            m_Image.LoadUrl(m_Asset.Stack.Image);

            UpdateLists();
        }

        private void UpdateLists()
        {
            // Clear not listed list
            ClearNotListedList();

            // Populate not listed items
            foreach (Listing stackListing in m_Asset.NotListed)
            {
                AssetNotListedObject item = Instantiate(m_NotListedObj, m_NotListedParent);
                item.gameObject.SetActive(true);
                item.Initialise(stackListing, OnSellButtonClicked); // Initialise the view with data
                m_NotListedViews.Add(item); // Add to the list of displayed attributes
            }
            m_EmptyNotListed.SetActive(m_Asset.NotListed.Count == 0);

            // Clear all existing listings
            ClearListings();

            // Populate listings
            foreach (Listing stackListing in m_Asset.Listings)
            {
                AssetListingObject item = Instantiate(m_ListingObj, m_ListingParent);
                item.gameObject.SetActive(true);
                item.Initialise(stackListing, OnCancelButtonClicked); // Initialise the view with data
                m_ListingViews.Add(item); // Add to the list of displayed attributes
            }
            m_EmptyListing.SetActive(m_Asset.Listings.Count == 0);
        }

        /// <summary>
        /// Handles the click event for the sell button.
        /// </summary>
        private async UniTask<bool> OnSellButtonClicked(Listing listing)
        {
            (bool result, string price) = await m_CustomDialog.ShowDialog(
                $"List {m_Asset.Stack.Name} for sale",
                "Enter your price below (in IMR):",
                "Confirm",
                negativeButtonText: "Cancel",
                showInputField: true
            );

            if (result)
            {
                decimal amount = Math.Floor(decimal.Parse(price) * (decimal)BigInteger.Pow(10, 18));
                string listingId = await PrepareListing(listing, $"{amount}");

                if (listingId != null)
                {
                    // TODO update to use get stack bundle by stack ID endpoint instead
                    // Locally remove token from not listed list
                    var listingToRemove = m_Asset.NotListed.FirstOrDefault(l => l.TokenId == listing.TokenId);
                    if (listingToRemove != null)
                    {
                        m_Asset.NotListed.Remove(listingToRemove);
                    }

                    // Locally add listing to listing
                    m_Asset.Listings.Insert(0, await GetListing(listingId));

                    UpdateLists();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the details for the listing
        /// </summary>
        private async UniTask<Listing> GetListing(string listingId) // TODO To replace with get stack by ID endpoint
        {
            try
            {
                using var client = new HttpClient();
                string url = $"{Config.BASE_URL}/v1/chains/{Config.CHAIN_NAME}/orders/listings/{listingId}";
                Debug.Log($"Get listing URL: {url}");

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    OrderResponse orderResponse = JsonUtility.FromJson<OrderResponse>(responseBody);

                    return new Listing(

                        listingId: orderResponse.result.id,
                        priceDetails: new PriceDetails
                        (
                            token: new PriceDetailsToken(new ERC20Token(symbol: "IMR", contractAddress: Contract.TOKEN, decimals: 18)),
                            amount: new PaymentAmount(orderResponse.result.buy[0].amount, orderResponse.result.buy[0].amount),
                            feeInclusiveAmount: new PaymentAmount(orderResponse.result.buy[0].amount, orderResponse.result.buy[0].amount), // Mocked
                            fees: orderResponse.result.fees.Select(fee => new Immutable.Search.Model.Fee(
                                fee.amount, Immutable.Search.Model.Fee.TypeEnum.ROYALTY, fee.recipient_address)).ToList()
                        ),
                        tokenId: orderResponse.result.sell[0].token_id,
                        creator: orderResponse.result.account_address,
                        amount: "1"
                    );
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
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
        /// Prepares the listing for the asset.
        /// </summary>
        /// <param name="listing">The asset to prepare for listing.</param>
        /// <param name="price">The price of the asset in smallest unit.</param>
        /// <returns>The listing ID is asset was successfully listed</returns>
        private async UniTask<string> PrepareListing(Listing asset, string price)
        {
            string address = SaveManager.Instance.WalletAddress;

            var data = new PrepareListingRequest
            {
                makerAddress = address,
                sell = new PrepareListingERC721Item
                {
                    contractAddress = Contract.SKIN,
                    tokenId = asset.TokenId,
                },
                buy = new PrepareListingERC20Item
                {
                    amount = price,
                    contractAddress = Contract.TOKEN,
                }
            };

            try
            {
                var json = JsonUtility.ToJson(data);
                Debug.Log($"json = {json}");

                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/v1/ts-sdk/v1/orderbook/prepareListing")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                using var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
                    await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing.", "OK");
                    return null;
                }

                string responseBody = await res.Content.ReadAsStringAsync();
                PrepareListingResponse response = JsonUtility.FromJson<PrepareListingResponse>(responseBody);

                // Send transaction if required
                var transaction = response.actions.FirstOrDefault(action => action.type == "TRANSACTION");
                if (transaction != null)
                {
                    var transactionResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(new TransactionRequest
                    {
                        to = transaction.populatedTransactions.to,
                        data = transaction.populatedTransactions.data,
                        value = "0"
                    });

                    if (transactionResponse.status != "1")
                    {
                        await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing.", "OK");
                        return null;
                    }
                }

                // Sign payload
                var signable = response.actions.FirstOrDefault(action => action.type == "SIGNABLE");
                if (signable != null)
                {
                    Debug.Log($"Sign: {JsonUtility.ToJson(signable.message)}");

                    signable.message.types.EIP712Domain = new List<NameType>
                    {
                        new NameType { name = "name", type = "string" },
                        new NameType { name = "version", type = "string" },
                        new NameType { name = "chainId", type = "uint256" },
                        new NameType { name = "verifyingContract", type = "address" }
                    };

                    var eip712TypedData = new EIP712TypedData
                    {
                        domain = signable.message.domain,
                        types = signable.message.types,
                        message = signable.message.value,
                        primaryType = "OrderComponents"
                    };

                    Debug.Log($"EIP712TypedData: {JsonUtility.ToJson(eip712TypedData)}");
                    string signature = await Passport.Instance.ZkEvmSignTypedDataV4(JsonUtility.ToJson(eip712TypedData));
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
                    return await ListAsset(signature, response, address);
                    // }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell: {ex.Message}");
                await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing", "OK");
            }

            return null;
        }

        /// <summary>
        /// Finalises the listing of the asset.
        /// </summary>
        /// <param name="signature">The signature for the listing.</param>
        /// <param name="preparedListing">The prepared listing data.</param>
        /// <param name="address">The wallet address of the user.</param>
        private async UniTask<string?> ListAsset(string signature, PrepareListingResponse preparedListing, string address)
        {
            var data = new CreateListingRequest
            {
                makerFees = new List<CreateListingFeeValue>(),
                orderComponents = preparedListing.orderComponents,
                orderHash = preparedListing.orderHash,
                orderSignature = signature
            };

            try
            {
                var json = JsonUtility.ToJson(data);
                Debug.Log($"json = {json}");

                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/v1/ts-sdk/v1/orderbook/createListing")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                using var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
                    string errorBody = await res.Content.ReadAsStringAsync();
                    Debug.Log($"Error: {errorBody}");
                    await m_CustomDialog.ShowDialog("Error", "Failed to list", "OK");
                    return null;
                }
                else
                {
                    string responseBody = await res.Content.ReadAsStringAsync();
                    CreateListingResponse response = JsonUtility.FromJson<CreateListingResponse>(responseBody);
                    Debug.Log($"Listing ID: {response.result.id}");

                    // Validate that listing is active
                    await ConfirmListingStatus(response.result.id, "ACTIVE");
                    return response.result.id;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to list: {ex.Message}");
                await m_CustomDialog.ShowDialog("Error", "Failed to list", "OK");
                return null;
            }
        }

        /// <summary>
        /// Cancels the listing of the asset.
        /// </summary>
        private async UniTask<bool> OnCancelButtonClicked(Listing listing)
        {
            Debug.Log($"Cancel listing {listing.ListingId}");

            string address = SaveManager.Instance.WalletAddress;
            var data = new CancelListingRequest
            {
                accountAddress = address,
                orderIds = new List<string> { listing.ListingId }
            };

            try
            {
                var json = JsonUtility.ToJson(data);
                Debug.Log($"json = {json}");

                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/v1/ts-sdk/v1/orderbook/cancelOrdersOnChain")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                using var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
                    await m_CustomDialog.ShowDialog("Error", "Failed to cancel listing", "OK");
                    return false;
                }

                string responseBody = await res.Content.ReadAsStringAsync();
                Debug.Log($"responseBody = {responseBody}");

                CancelListingResponse response = JsonUtility.FromJson<CancelListingResponse>(responseBody);
                if (response?.cancellationAction.populatedTransaction.to != null)
                {
                    var transactionResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(new TransactionRequest()
                    {
                        to = response.cancellationAction.populatedTransaction.to, // Immutable seaport contract
                        data = response.cancellationAction.populatedTransaction.data, // fd9f1e10 cancel
                        value = "0"
                    });

                    if (transactionResponse.status == "1")
                    {
                        // Validate that listing has been cancelled
                        await ConfirmListingStatus(listing.ListingId, "CANCELLED");

                        // TODO update to use get stack bundle by stack ID endpoint instead
                        // Locally remove listing
                        var listingToRemove = m_Asset.Listings.FirstOrDefault(l => l.ListingId == listing.ListingId);
                        if (listingToRemove != null)
                        {
                            m_Asset.Listings.Remove(listingToRemove);
                        }

                        // Locally add asset to not listed list
                        m_Asset.NotListed.Insert(0, listing); // TODO

                        UpdateLists();

                        return true;
                    }
                    else
                    {
                        await m_CustomDialog.ShowDialog("Error", "Failed to cancel listing", "OK");
                        return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                await m_CustomDialog.ShowDialog("Error", "Failed to cancel listing", "OK");
                return false;
            }
        }

        /// <summary>
        /// Polls the listing status until it transitions to the given status or the operation times out after 1 minute.
        /// </summary>
        private async UniTask ConfirmListingStatus(string listingId, string status)
        {
            Debug.Log($"Confirming listing {listingId} is {status}...");

            bool conditionMet = await PollingHelper.PollAsync(
                $"https://api.dev.immutable.com/v1/chains/imtbl-zkevm-devnet/orders/listings/{listingId}",
                (responseBody) =>
                {
                    ListingResponse listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);
                    return listingResponse.result?.status.name == status;
                });

            if (conditionMet)
            {
                await m_CustomDialog.ShowDialog("Success", $"Listing is {status.ToLower()}.", "OK");
            }
            else
            {
                await m_CustomDialog.ShowDialog("Error", $"Failed to confirm if listing is {status.ToLower()}.", "OK");
            }
        }

        private void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }

        /// <summary>
        /// Removes all the attribute views
        /// </summary>
        private void ClearAttributes()
        {
            foreach (AttributeView attribute in m_Attributes)
            {
                Destroy(attribute.gameObject);
            }
            m_Attributes.Clear();
        }

        /// <summary>
        /// Removes all the not for sale views
        /// </summary>
        private void ClearNotListedList()
        {
            foreach (AssetNotListedObject listing in m_NotListedViews)
            {
                Destroy(listing.gameObject);
            }
            m_NotListedViews.Clear();
        }

        /// <summary>
        /// Removes all the listing views
        /// </summary>
        private void ClearListings()
        {
            foreach (AssetListingObject listing in m_ListingViews)
            {
                Destroy(listing.gameObject);
            }
            m_ListingViews.Clear();
        }

        /// <summary>
        /// Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_NameText.text = "";
            m_CollectionText.text = ""; ;

            m_Asset = null;
            ClearAttributes();
            ClearNotListedList();
            ClearListings();
        }
    }
}
