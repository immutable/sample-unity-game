using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Numerics;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using Immutable.Passport;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Immutable.Passport.Model;

namespace HyperCasual.Runner
{
    public class OrderDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private TextMeshProUGUI m_NameText = null;
        [SerializeField] private TextMeshProUGUI m_CollectionText = null;
        [SerializeField] private TextMeshProUGUI m_AmountText = null;
        [SerializeField] private Transform m_AttributesListParent = null;
        [SerializeField] private AttributeView m_AttributeObj = null;
        private List<AttributeView> m_AttributeViews = new List<AttributeView>();
        [SerializeField] private ImageUrlObject m_Image = null;
        [SerializeField] private GameObject m_Progress = null;
        [SerializeField] private CustomDialog m_CustomDialog;

        [SerializeField] private Transform m_ListingParent = null;
        private List<ListingObject> m_ListingViews = new List<ListingObject>();
        [SerializeField] private ListingObject m_ListingObj = null;

        private StacksResult m_Order;
        private Listing m_Listing;

        async void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Hide template attribute object
            m_ListingObj.gameObject.SetActive(false); // Hide listing template object

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

            // Gets the player's balance
            m_Balance.UpdateBalance();
        }

        /// <summary>
        /// Initialises the UI based on the order
        /// </summary>
        public async void Initialise(StacksResult order)
        {
            m_Order = order;
            UpdateData();

            // Hide progress
            m_Progress.SetActive(false);
        }

        /// <summary>
        /// Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            m_NameText.text = m_Order.stack.name;
            m_CollectionText.text = $"Collection: {m_Order.stack.contract_address}";

            // Clears all existing attributes
            ClearAttributes();

            // Populate attributes
            foreach (AssetAttribute attribute in m_Order.stack.attributes)
            {
                AttributeView newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent); // Create a new asset object
                newAttribute.gameObject.SetActive(true);
                newAttribute.Initialise(attribute); // Initialise the view with data
                m_AttributeViews.Add(newAttribute); // Add to the list of displayed attributes
            }

            // Download and display the image
            m_Image.LoadUrl(m_Order.stack.image);

            // Clear all existing listings
            ClearListings();

            // Populate listings
            foreach (StackListing stackListing in m_Order.listings)
            {
                ListingObject newListing = Instantiate(m_ListingObj, m_ListingParent);
                newListing.gameObject.SetActive(true);
                newListing.Initialise(stackListing, OnBuyButtonClick); // Initialise the view with data
                m_ListingViews.Add(newListing); // Add to the list of displayed attributes
            }
        }

        /// <summary>
        /// Removes all the attribute views
        /// </summary>
        private void ClearAttributes()
        {
            foreach (AttributeView attribute in m_AttributeViews)
            {
                Destroy(attribute.gameObject);
            }
            m_AttributeViews.Clear();
        }

        /// <summary>
        /// Removes all the listing views
        /// </summary>
        private void ClearListings()
        {
            foreach (ListingObject listing in m_ListingViews)
            {
                Destroy(listing.gameObject);
            }
            m_ListingViews.Clear();
        }

        /// <summary>
        /// Handles the buy button click event. Sends a request to fulfil an order, 
        /// processes the response, and updates the UI accordingly.
        /// </summary>
        private async UniTask<bool> OnBuyButtonClick(StackListing listing)
        {
            string address = SaveManager.Instance.WalletAddress;
            var data = new FulfullOrderRequest
            {
                takerAddress = address,
                listingId = listing.listing_id,
                takerFees = listing.fees.Select(fee => new FulfullOrderRequestFee
                {
                    amount = fee.amount,
                    recipientAddress = fee.recipient_address
                }).ToArray()
            };

            try
            {
                var json = JsonUtility.ToJson(data);
                Debug.Log($"json = {json}");

                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:8080/v1/ts-sdk/v1/orderbook/fulfillOrder")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                using var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                {
                    string errorBody = await res.Content.ReadAsStringAsync();
                    Debug.Log($"errorBody = {errorBody}");
                    await m_CustomDialog.ShowDialog("Error", "Failed to buy", "OK");
                    return false;
                }

                string responseBody = await res.Content.ReadAsStringAsync();
                FulfullOrderResponse response = JsonUtility.FromJson<FulfullOrderResponse>(responseBody);
                if (response.transactions != null)
                {
                    foreach (Transaction transaction in response.transactions)
                    {
                        string transactionHash = await Passport.Instance.ZkEvmSendTransaction(new TransactionRequest
                        {
                            to = transaction.to, // Immutable seaport contract
                            data = transaction.data, // 87201b41 fulfillAvailableAdvancedOrders
                            value = "0"
                        });
                    }

                    // Validate that order is fulfilled
                    await ConfirmListingStatus();
                    m_Balance.UpdateBalance(); // Update user's balance on successful buy

                    // TODO update to use get stack bundle by stack ID endpoint later
                    // Locally update stack listing
                    var listingToRemove = m_Order.listings.FirstOrDefault(l => l.listing_id == listing.listing_id);
                    if (listingToRemove != null)
                    {
                        m_Order.listings.Remove(listingToRemove);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to buy: {ex.Message}");
                await m_CustomDialog.ShowDialog("Error", "Failed to buy", "OK");
                return false;
            }
        }

        /// <summary>
        /// Polls the order status until it transitions to FULFILLED or the operation times out after 1 minute.
        /// </summary>
        private async UniTask ConfirmListingStatus()
        {
            Debug.Log($"Confirming order is filled...");

            bool conditionMet = await PollingHelper.PollAsync(
                $"https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings/{m_Order.listings[0].listing_id}",
                (responseBody) =>
                {
                    ListingResponse listingResponse = JsonUtility.FromJson<ListingResponse>(responseBody);
                    return listingResponse.result?.status.name == "FILLED";
                });

            if (conditionMet)
            {
                await m_CustomDialog.ShowDialog("Success", $"Order is filled.", "OK");
            }
            else
            {
                await m_CustomDialog.ShowDialog("Error", $"Failed to confirm if order is filled.", "OK");
            }
        }

        /// <summary>
        /// Handles the back button click
        /// </summary>
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
            m_CollectionText.text = "";

            m_Order = null;
            ClearAttributes();
            ClearListings();
        }
    }
}