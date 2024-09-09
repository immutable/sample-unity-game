using System;
using System.Collections;
using System.Collections.Generic;
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
using Immutable.Search.Model;

namespace HyperCasual.Runner
{
    public class ListingObject : View
    {
        [SerializeField] private TextMeshProUGUI m_AmountText = null;
        [SerializeField] private TextMeshProUGUI m_TokenIdText = null;
        [SerializeField] private HyperCasualButton m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_PlayersListingText = null;
        [SerializeField] private GameObject m_Progress = null;
        private Listing m_Listing;
        private Func<Listing, UniTask<bool>> m_OnBuy;

        async void OnEnable()
        {
        }

        /// <summary>
        /// Initialises the UI based on the order
        /// </summary>
        public async void Initialise(Listing listing, Func<Listing, UniTask<bool>> onBuy)
        {
            m_Listing = listing;
            m_OnBuy = onBuy;
            UpdateData();

            // Check if asset is the player's asset
            string address = SaveManager.Instance.WalletAddress;
            // bool isPlayersAsset = m_Listing.account_address == address; // TODO added myself
            // m_PlayersListingText.gameObject.SetActive(isPlayersAsset);
            // m_BuyButton.gameObject.SetActive(!isPlayersAsset);

            // Hide progress
            m_Progress.SetActive(false);

            // Set listeners to button
            m_BuyButton.RemoveListener(OnBuyButtonClick);
            m_BuyButton.AddListener(OnBuyButtonClick);
        }

        /// <summary>
        /// Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            m_TokenIdText.text = $"Token ID: {m_Listing.TokenId}";

            // Price
            string amount = m_Listing.PriceDetails.Amount.Value;
            decimal quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
            m_AmountText.text = $"{quantity} IMR";
        }

        /// <summary>
        /// Handles the buy button click event.
        /// </summary>
        private async void OnBuyButtonClick()
        {
            m_BuyButton.gameObject.SetActive(false);
            m_Progress.SetActive(true);

            bool success = await m_OnBuy(m_Listing);

            m_PlayersListingText.gameObject.SetActive(success);
            m_BuyButton.gameObject.SetActive(!success);
            m_Progress.SetActive(false);
        }

        /// <summary>
        /// Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_TokenIdText.text = "";
            m_AmountText.text = "";

            m_Listing = null;
        }
    }
}