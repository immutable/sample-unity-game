using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http;
using Immutable.Passport;
using Cysharp.Threading.Tasks;
using System.Numerics;

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

        // If there's an error minting, these values will be used when the player clicks the "Try again" button
        private bool mintedFox = false;
        private bool mintedCoins = false;

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

            // Reset values
            mintedFox = false;
            mintedCoins = false;

            Mint();
        }

        private async void Mint()
        {
            try
            {
                ShowMintingMessage();
                ShowLoading(true);
                ShowError(false);
                ShowNextButton(false);

                // Mint fox if not minted yet
                if (!mintedFox)
                {
                    mintedFox = await MintFox();
                }
                // Mint coins if not minted yet
                if (!mintedCoins)
                {
                    mintedCoins = await MintCoins();
                }

                // Show minted message if minted both fox and coins successfully
                if (mintedFox && mintedCoins)
                {
                    ShowMintedMessage();
                }
                ShowLoading(false);
                // Show error if failed to mint fox or coins
                ShowError(!mintedFox || !mintedCoins);
                // Show next button is minted both fox and coins successfully
                ShowNextButton(mintedFox && mintedCoins);
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

        /// <summary>
        /// Mints a fox (i.e. Immutable Runner Fox) to the player's wallet
        /// </summary>
        /// <returns>True if minted a fox successfully to player's wallet. Otherwise, false.</returns>
        private async UniTask<bool> MintFox()
        {
            try
            {
                var address = SaveManager.Instance.WalletAddress; // Get the player's wallet address to mint the fox to

                if (address != null) return await ApiService.MintFox(address);

                return false;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to mint fox: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Mints collected coins (i.e. Immutable Runner Token) to the player's wallet
        /// </summary>
        /// <returns>True if minted coins successfully to player's wallet. Otherwise, false.</returns>
        private async UniTask<bool> MintCoins()
        {
            try
            {
                int coinsCollected = GetNumCoinsCollected(); // Get number of coins collected
                if (coinsCollected == 0) // Don't mint any coins if player did not collect any
                {
                    return true;
                }

                var address = SaveManager.Instance.WalletAddress; // Get the player's wallet address to mint the coins to
                if (address != null)
                {
                    return await ApiService.MintTokens(coinsCollected, address);
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to mint coins: {ex.Message}");
                return false;
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
            m_Loading?.gameObject.SetActive(show);
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

        private async void OnWalletClicked()
        {
            // Get the player's wallet address to mint the fox to
            string address = SaveManager.Instance.WalletAddress;
            if (SaveManager.Instance.ZkEvm)
            {
                // Show the player's tokens on the block explorer page.
                Application.OpenURL($"https://explorer.testnet.immutable.com/address/{address}?tab=tokens");
            }
            else
            {
                Application.OpenURL($"{Config.SERVER_BASE_URL}/wallet?user={address}");
            }
        }
    }
}
