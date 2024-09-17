using System;
using System.Numerics;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Search.Model;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class ListingObject : View
    {
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_TokenIdText;
        [SerializeField] private HyperCasualButton m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_PlayersListingText;
        [SerializeField] private GameObject m_Progress;
        private Listing m_Listing;
        private Func<Listing, UniTask<bool>> m_OnBuy;

        private async void OnEnable()
        {
        }

        /// <summary>
        ///     Cleans up data
        /// </summary>
        private void OnDisable()
        {
            m_TokenIdText.text = "";
            m_AmountText.text = "";

            m_Listing = null;
        }

        /// <summary>
        ///     Initialises the UI based on the order
        /// </summary>
        public async void Initialise(Listing listing, Func<Listing, UniTask<bool>> onBuy)
        {
            m_Listing = listing;
            m_OnBuy = onBuy;
            UpdateData();

            // Check if asset is the player's asset
            var address = SaveManager.Instance.WalletAddress;
            var isPlayersAsset = m_Listing.Creator == address;
            m_PlayersListingText.gameObject.SetActive(isPlayersAsset);
            m_BuyButton.gameObject.SetActive(!isPlayersAsset);

            // Hide progress
            m_Progress.SetActive(false);

            // Set listeners to button
            m_BuyButton.RemoveListener(OnBuyButtonClick);
            m_BuyButton.AddListener(OnBuyButtonClick);
        }

        /// <summary>
        ///     Updates the text fields with asset data.
        /// </summary>
        private async void UpdateData()
        {
            m_TokenIdText.text = $"Token ID: {m_Listing.TokenId}";

            // Price
            var amount = m_Listing.PriceDetails.Amount.Value;
            var quantity = (decimal)BigInteger.Parse(amount) / (decimal)BigInteger.Pow(10, 18);
            m_AmountText.text = $"{quantity} IMR";
        }

        /// <summary>
        ///     Handles the buy button click event.
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