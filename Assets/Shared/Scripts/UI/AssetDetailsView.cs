using System;
using System.Collections.Generic;
using System.Numerics;
using System.Net.Http;
using System.Linq;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Immutable.Passport;
using Immutable.Passport.Model;

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
        private StacksResult m_Asset;

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
        public async void Initialise(StacksResult asset)
        {
            m_Asset = asset;

            m_NameText.text = m_Asset.stack.name;
            m_CollectionText.text = $"Collection: {m_Asset.stack.contract_address}";

            // Clear existing attributes
            ClearAttributes();

            // Populate attributes
            foreach (AssetAttribute attribute in m_Asset.stack.attributes)
            {
                AttributeView newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent);
                newAttribute.gameObject.SetActive(true);
                newAttribute.Initialise(attribute);
                m_Attributes.Add(newAttribute);
            }

            // Download and display the image
            m_Image.LoadUrl(m_Asset.stack.image);

            UpdateLists();
        }

        private void UpdateLists()
        {
            // Clear not listed list
            ClearNotListedList();

            // Populate not listed items
            foreach (StackListing stackListing in m_Asset.notListed)
            {
                AssetNotListedObject item = Instantiate(m_NotListedObj, m_NotListedParent);
                item.gameObject.SetActive(true);
                item.Initialise(stackListing, OnSellButtonClicked); // Initialise the view with data
                m_NotListedViews.Add(item); // Add to the list of displayed attributes
            }
            m_EmptyNotListed.SetActive(m_Asset.notListed.Count == 0);

            // Clear all existing listings
            ClearListings();

            // Populate listings
            foreach (StackListing stackListing in m_Asset.listings)
            {
                AssetListingObject item = Instantiate(m_ListingObj, m_ListingParent);
                item.gameObject.SetActive(true);
                item.Initialise(stackListing, OnCancelButtonClicked); // Initialise the view with data
                m_ListingViews.Add(item); // Add to the list of displayed attributes
            }
            m_EmptyListing.SetActive(m_Asset.listings.Count == 0);
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
        /// Handles the click event for the sell button.
        /// </summary>
        private async UniTask<bool> OnSellButtonClicked(StackListing listing)
        {
            (bool result, string price) = await m_CustomDialog.ShowDialog(
                $"List {m_Asset.stack.name} for sale",
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
                    // Locally remove token from not listed list
                    var listingToRemove = m_Asset.notListed.FirstOrDefault(l => l.token_id == listing.token_id);
                    if (listingToRemove != null)
                    {
                        m_Asset.notListed.Remove(listingToRemove);
                    }

                    // Locally add listing to listing
                    m_Asset.listings.Insert(0, await GetListing(listingId));

                    UpdateLists();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the details for the listing
        /// </summary>
        private async UniTask<StackListing> GetListing(string listingId) // TODO To replace with get stack by ID endpoint
        {
            try
            {
                using var client = new HttpClient();
                string url = $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings/{listingId}";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    OrderResponse orderResponse = JsonUtility.FromJson<OrderResponse>(responseBody);

                    return new StackListing
                    {
                        listing_id = orderResponse.result.id,
                        price = new Price
                        {
                            token = new Token
                            {
                                type = "ERC20",
                                symbol = "IMR"
                            },
                            amount = new Amount
                            {
                                value = orderResponse.result.buy[0].amount
                            }
                        },
                        token_id = orderResponse.result.sell[0].token_id,
                        quantity = 1,
                        account_address = orderResponse.result.account_address,
                        fees = orderResponse.result.fees
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to check sale status: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Prepares the listing for the asset.
        /// </summary>
        /// <param name="listing">The asset to prepare for listing.</param>
        /// <param name="price">The price of the asset in smallest unit.</param>
        /// <returns>The listing ID is asset was successfully listed</returns>
        private async UniTask<string> PrepareListing(StackListing asset, string price)
        {
            try
            {
                string address = SaveManager.Instance.WalletAddress;
                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("offererAddress", address),
                    new KeyValuePair<string, string>("amount", price),
                    new KeyValuePair<string, string>("tokenId", asset.token_id)
                };
                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/prepareListing/skin") { Content = new FormUrlEncodedContent(nvc) };
                using var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
                    await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing.", "OK");
                    return null;
                }

                string responseBody = await res.Content.ReadAsStringAsync();
                PrepareListingResponse response = JsonUtility.FromJson<PrepareListingResponse>(responseBody);

                if (response.transactionToSend?.to != null)
                {
                    var transactionResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(new TransactionRequest
                    {
                        to = response.transactionToSend.to,
                        data = response.transactionToSend.data,
                        value = "0"
                    });

                    if (transactionResponse.status != "1")
                    {
                        await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing.", "OK");
                        return null;
                    }
                }

                if (response.toSign != null && response.preparedListing != null)
                {
                    // Prompt for signature
                    Debug.Log($"Sign: {response.toSign}");
                    (bool result, string signature) = await m_CustomDialog.ShowDialog(
                        "Confirm listing",
                        "Enter signed payload:",
                        "Confirm",
                        negativeButtonText: "Cancel",
                        showInputField: true
                    );
                    if (result)
                    {
                        return await ListAsset(signature, response.preparedListing, address);
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to sell: {ex.Message}");
                await m_CustomDialog.ShowDialog("Error", "Failed to prepare listing", "OK");
                return null;
            }
        }

        /// <summary>
        /// Finalises the listing of the asset.
        /// </summary>
        /// <param name="signature">The signature for the listing.</param>
        /// <param name="preparedListing">The prepared listing data.</param>
        /// <param name="address">The wallet address of the user.</param>
        private async UniTask<string?> ListAsset(string signature, string preparedListing, string address)
        {
            try
            {
                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("signature", signature),
                    new KeyValuePair<string, string>("preparedListing", preparedListing)
                };
                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6060/createListing/skin") { Content = new FormUrlEncodedContent(nvc) };
                using var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
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
        private async UniTask<bool> OnCancelButtonClicked(StackListing listing)
        {
            Debug.Log($"Cancel listing {listing.listing_id}");

            try
            {
                string address = SaveManager.Instance.WalletAddress;
                var nvc = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("offererAddress", address),
                    new KeyValuePair<string, string>("listingId", listing.listing_id),
                    new KeyValuePair<string, string>("type", "hard")
                };

                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost:6060/cancelListing/skin")
                {
                    Content = new FormUrlEncodedContent(nvc)
                };

                using var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
                    await m_CustomDialog.ShowDialog("Error", "Failed to cancel listing", "OK");
                    return false;
                }

                string responseBody = await res.Content.ReadAsStringAsync();

                TransactionToSend response = JsonUtility.FromJson<TransactionToSend>(responseBody);
                if (response?.to != null)
                {
                    var transactionResponse = await Passport.Instance.ZkEvmSendTransactionWithConfirmation(new TransactionRequest()
                    {
                        to = response.to, // Immutable seaport contract
                        data = response.data, // fd9f1e10 cancel
                        value = "0"
                    });

                    if (transactionResponse.status == "1")
                    {
                        // Validate that listing has been cancelled
                        await ConfirmListingStatus(listing.listing_id, "CANCELLED");

                        // Locally remove listing
                        var listingToRemove = m_Asset.listings.FirstOrDefault(l => l.listing_id == listing.listing_id);
                        if (listingToRemove != null)
                        {
                            m_Asset.listings.Remove(listingToRemove);
                        }

                        // Locally add asset to not listed list
                        m_Asset.notListed.Insert(0, listing);

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
                $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings/{listingId}",
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
