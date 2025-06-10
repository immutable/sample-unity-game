using System;
using System.Numerics;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Api.ZkEvm.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Represents a listing object that displays asset data and handles user interaction with buy functionality
    ///     in the Marketplace Asset Details View.
    /// </summary>
    public class ListingObject : View
    {
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private HyperCasualButton m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_PlayersListingText;
        [SerializeField] private GameObject m_Progress;

        private Listing m_Listing;
        private Func<Listing, UniTask<bool>> m_OnBuy;

        /// <summary>
        ///     Clears any displayed listing data when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            m_TokenIdText.text = string.Empty;
            m_PriceText.text = string.Empty;
            m_AmountText.text = string.Empty;
            m_Listing = null;
        }

        /// <summary>
        ///     Initialises the UI with the given listing details.
        /// </summary>
        /// <param name="listing">The listing to display.</param>
        /// <param name="onBuy">The action to execute when the buy button is pressed.</param>
        public async void Initialise(Listing listing, Func<Listing, UniTask<bool>> onBuy)
        {
            m_Listing = listing;
            m_OnBuy = onBuy;

            UpdateData();

            // Check if the asset belongs to the player.
            var isPlayersAsset = m_Listing.Creator == SaveManager.Instance.WalletAddress;
            m_PlayersListingText.gameObject.SetActive(isPlayersAsset);
            m_BuyButton.gameObject.SetActive(!isPlayersAsset);

            // Hide the progress indicator initially.
            m_Progress.SetActive(false);

            // Ensure button listeners are correctly set.
            m_BuyButton.RemoveListener(OnBuyButtonClick);
            m_BuyButton.AddListener(OnBuyButtonClick);
        }

        /// <summary>
        ///     Updates the UI text fields with the listing’s token ID, amount, and price details.
        /// </summary>
        private async void UpdateData()
        {
            m_TokenIdText.text = $"Token ID: {m_Listing.TokenId}";
            m_AmountText.text = $"Amount: {m_Listing.Amount}";

            var rawAmount = m_Listing.PriceDetails.Amount;
            var quantity = (decimal)BigInteger.Parse(rawAmount) / (decimal)BigInteger.Pow(10, 18);
            m_PriceText.text = $"{quantity} IMR";
        }

        /// <summary>
        ///     Handles the buy button click event by triggering the purchase flow.
        ///     Displays progress and adjusts UI based on the result.
        /// </summary>
        private async void OnBuyButtonClick()
        {
            m_BuyButton.gameObject.SetActive(false);
            m_Progress.SetActive(true);

            var success = await m_OnBuy(m_Listing);

            m_PlayersListingText.gameObject.SetActive(success);
            m_BuyButton.gameObject.SetActive(!success);
            m_Progress.SetActive(false);
        }
    }
}