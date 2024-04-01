using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains celebration screen functionalities
    /// </summary>
    public class MintScreen : View
    {

        [SerializeField]
        TextMeshProUGUI m_Title;
        [SerializeField]
        GameObject m_Loading;
        [SerializeField]
        TextMeshProUGUI m_CheckoutWalletMessage;
        [SerializeField]
        TextMeshProUGUI m_WalletText;
        [SerializeField]
        HyperCasualButton m_NextButton;
        [SerializeField]
        TextMeshProUGUI m_ErrorMessage;
        [SerializeField]
        HyperCasualButton m_TryAgainButton;
        [SerializeField]
        AbstractGameEvent m_NextEvent;
        [SerializeField]
        HyperCasualButton m_WalletButton;

        public void OnEnable()
        {
            // Set listener to 'Next' button
            m_NextButton.RemoveListener(OnNextButtonClicked);
            m_NextButton.AddListener(OnNextButtonClicked);

            // Set listener to "Try again" button
            m_TryAgainButton.RemoveListener(Mint);
            m_TryAgainButton.AddListener(Mint);

            // Set listener to 'Wallet' button
            m_WalletButton.RemoveListener(OnWalletClicked);
            m_WalletButton.AddListener(OnWalletClicked);

            Mint();
        }

        private void Mint()
        {
            try
            {
                ShowMintingMessage();
                ShowLoading(true);
                ShowError(false);
                ShowNextButton(false);

                // Mint

                ShowMintedMessage();
                ShowLoading(false);
                ShowError(false);
                ShowNextButton(true);
            }
            catch (Exception ex)
            {
                // Failed to mint, let the player try again
                Debug.Log($"Failed to mint: {ex.Message}");
                ShowLoading(false);
                ShowError(true);
                ShowNextButton(false);
            }
        }

        private void OnNextButtonClicked()
        {
            m_NextEvent.Raise();
        }

        private void ShowNextButton(bool show)
        {
            m_NextButton.gameObject.SetActive(show);
        }

        private void ShowLoading(bool show)
        {
            m_Loading.gameObject.SetActive(show);
        }

        private void ShowError(bool show)
        {
            m_ErrorMessage.gameObject.SetActive(show);
            m_TryAgainButton.gameObject.SetActive(show);
        }

        private void ShowCheckoutWallet(bool show)
        {
            m_CheckoutWalletMessage.gameObject.SetActive(show);
            m_WalletText.gameObject.SetActive(show);
        }

        private void ShowMintingMessage()
        {
            ShowCheckoutWallet(false);
            // Get number of coins col
            int numCoins = GetNumCoinsCollected();
            if (numCoins > 0)
            {
                m_Title.text = $"Let's mint the {numCoins} coin{(numCoins > 1 ? "s" : "")} you've collected and a fox to your wallet";
            }
            else
            {
                m_Title.text = "Let's mint a fox to your wallet!";
            }
        }

        /// <summary>
        /// Get the number of coins the player collected from the Level Complete Screen
        /// </summary>
        private int GetNumCoinsCollected()
        {
            LevelCompleteScreen levelCompleteScreen = UIManager.Instance.GetView<LevelCompleteScreen>();
            return levelCompleteScreen.CoinCount;
        }

        private void ShowMintedMessage()
        {
            ShowCheckoutWallet(true);
            int numCoins = GetNumCoinsCollected();
            if (numCoins > 0)
            {
                m_Title.text = $"You now own {numCoins} coin{(numCoins > 1 ? "s" : "")} and a fox";
            }
            else
            {
                m_Title.text = "You now own a fox!";
            }
        }

        private void OnWalletClicked()
        {
        }
    }
}
