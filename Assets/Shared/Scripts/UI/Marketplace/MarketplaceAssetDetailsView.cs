using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Api.ZkEvm.Model;
using Immutable.Orderbook.Client;
using Immutable.Orderbook.Model;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class MarketplaceAssetDetailsView : View
    {
        [SerializeField] private HyperCasualButton m_BackButton;
        [SerializeField] private BalanceObject m_Balance;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_Count;
        [SerializeField] private TextMeshProUGUI m_CollectionText;
        [SerializeField] private TextMeshProUGUI m_FloorPriceText;
        [SerializeField] private TextMeshProUGUI m_LastTradePriceText;
        [SerializeField] private Transform m_AttributesListParent;
        [SerializeField] private AttributeView m_AttributeObj;
        [SerializeField] private ImageUrlObject m_Image;
        [SerializeField] private CustomDialog m_CustomDialog;

        [SerializeField] private Transform m_ListingParent;
        [SerializeField] private ListingObject m_ListingObj;
        [SerializeField] private GameObject m_EmptyListingText;
        private readonly List<AttributeView> m_AttributeViews = new();
        private readonly List<ListingObject> m_ListingViews = new();

        private Listing m_Listing;

        private StackBundle m_Order;

        private async void OnEnable()
        {
            m_AttributeObj.gameObject.SetActive(false); // Hide template attribute object
            m_ListingObj.gameObject.SetActive(false); // Hide listing template object

            m_BackButton.RemoveListener(OnBackButtonClick);
            m_BackButton.AddListener(OnBackButtonClick);

#pragma warning disable CS4014
            m_Balance.UpdateBalance();
#pragma warning restore CS4014
        }

        /// <summary>
        ///     Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_NameText.text = "";
            m_CollectionText.text = "";
            m_FloorPriceText.text = "";
            m_LastTradePriceText.text = "";

            m_Order = null;
            ClearAttributes();
            ClearListings();
        }

        /// <summary>
        ///     Initialises the UI based on the order
        /// </summary>
        public async void Initialise(StackBundle order)
        {
            m_Order = order;
            UpdateData();
        }

        /// <summary>
        ///     Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            m_NameText.text = m_Order.Stack.Name;
            m_Count.text = $"{m_Order.StackCount} items";
            m_CollectionText.text = $"Collection: {m_Order.Stack.ContractAddress}";

            // Floor price
            if (m_Order.Market?.FloorListing != null)
            {
                var amount = m_Order.Market.FloorListing.PriceDetails.Amount;

                var quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                m_FloorPriceText.text = $"Floor price: {quantity} IMR";
            }
            else
            {
                m_FloorPriceText.text = "Floor price: N/A";
            }

            // Last trade price
            if (m_Order.Market?.LastTrade?.PriceDetails?.Count > 0)
            {
                var amount = m_Order.Market.LastTrade.PriceDetails[0].Amount;

                var quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
                m_LastTradePriceText.text = $"Last trade price: {quantity} IMR";
            }
            else
            {
                m_LastTradePriceText.text = "Last trade price: N/A";
            }

            // Clears all existing attributes
            ClearAttributes();

            // Populate attributes
            var attributes = m_Order.Stack?.Attributes ?? new List<NFTMetadataAttribute>();
            foreach (var attribute in attributes)
            {
                var newAttribute = Instantiate(m_AttributeObj, m_AttributesListParent); // Create a new asset object
                newAttribute.gameObject.SetActive(true);
                newAttribute.Initialise(attribute); // Initialise the view with data
                m_AttributeViews.Add(newAttribute); // Add to the list of displayed attributes
            }

            // Download and display the image
            m_Image.LoadUrl(m_Order.Stack.Image);

            // Clear all existing listings
            ClearListings();

            // Populate listings
            foreach (var stackListing in m_Order.Listings)
            {
                var newListing = Instantiate(m_ListingObj, m_ListingParent);
                newListing.gameObject.SetActive(true);
                newListing.Initialise(stackListing, OnBuyButtonClick); // Initialise the view with data
                m_ListingViews.Add(newListing); // Add to the list of displayed attributes
            }

            m_EmptyListingText.SetActive(m_Order.Listings.Count == 0);
        }

        /// <summary>
        ///     Removes all the attribute views
        /// </summary>
        private void ClearAttributes()
        {
            foreach (var attribute in m_AttributeViews) Destroy(attribute.gameObject);
            m_AttributeViews.Clear();
        }

        /// <summary>
        ///     Removes all the listing views
        /// </summary>
        private void ClearListings()
        {
            foreach (var listing in m_ListingViews) Destroy(listing.gameObject);
            m_ListingViews.Clear();
        }

        /// <summary>
        ///     Handles the buy button click event.
        /// </summary>
        private async UniTask<bool> OnBuyButtonClick(Listing listing)
        {
            try
            {
                var takerFees = listing.PriceDetails.Fees
                    .Select(fee => new FulfillOrderRequestTakerFeesInner(fee.Amount, fee.RecipientAddress)).ToList();

                await FulfilOrderUseCase.Instance.ExecuteOrder(listing.ListingId, takerFees);

#pragma warning disable CS4014
                m_Balance.UpdateBalance();
#pragma warning restore CS4014

                // Locally update stack listing
                m_Order.Listings.RemoveAll(l => l.ListingId == listing.ListingId);

                return true;
            }
            catch (ApiException e)
            {
                Debug.LogError("Exception when calling: " + e.Message);
                Debug.LogError("Status Code: " + e.ErrorCode);
                var errorModel = JsonConvert.DeserializeObject<ErrorModel>($"{e.ErrorContent}");
                await m_CustomDialog.ShowDialog("Error", errorModel.message, "OK");
                Debug.LogError(e.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to buy: {ex.Message}");
                await m_CustomDialog.ShowDialog("Error", "Failed to buy", "OK");
            }

            return false;
        }

        private void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }
    }
}