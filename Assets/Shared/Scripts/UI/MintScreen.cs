using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using Immutable.Passport;
using Immutable.Passport.Model;

[Serializable]
public class MintResult
{
    public string token_id;
    public string contract_address;
    public string tx_id;
}

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
                    MintResult mintResult = await MintFox();

                    // Show minted message if minted fox successfully
                    ShowMintedMessage();
                }
            }
            catch (Exception ex)
            {
                // Failed to mint, let the player try again
                Debug.Log($"Failed to mint or transfer: {ex.Message}");
            }
            ShowLoading(false);

            // Show error if failed to mint fox
            ShowError(!mintedFox);

            // Show next button if fox minted successfully
            ShowNextButton(mintedFox);
        }

        /// <summary>
        /// Gets the wallet address of the player.
        /// </summary>
        private async UniTask<string> GetWalletAddress()
        {
            string address = await Passport.Instance.GetAddress();
            return address;
        }

        /// <summary>
        /// Mints a fox (i.e. Immutable Runner Fox) to the player's wallet
        /// </summary>
        /// <returns>True if minted a fox successfully to player's wallet. Otherwise, false.</returns>
        private async UniTask<MintResult> MintFox()
        {
            Debug.Log("Minting fox...");
            try
            {
                string address = await GetWalletAddress(); // Get the player's wallet address to mint the fox to

                if (address != null)
                {
                    var nvc = new List<KeyValuePair<string, string>>
                {
                    // Set 'to' to the player's wallet address
                    new KeyValuePair<string, string>("to", address)
                };
                    using var client = new HttpClient();
                    string url = $"http://localhost:3000/mint/fox"; // Endpoint to mint fox
                    using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(nvc) };
                    using var res = await client.SendAsync(req);

                    // Parse JSON and extract token_id
                    string content = await res.Content.ReadAsStringAsync();
                    Debug.Log($"Mint fox response: {content}");

                    MintResult mintResult = JsonUtility.FromJson<MintResult>(content);
                    Debug.Log($"Minted fox with token_id: {mintResult.token_id}");

                    mintedFox = res.IsSuccessStatusCode;
                    return mintResult;
                }

                mintedFox = false;
                return null;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to mint fox: {ex.Message}");
                mintedFox = false;
                return null;
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
            m_Title.text = "Let's mint a fox to your wallet!";
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
            m_Title.text = "You now own a fox!";
        }

        private async void OnWalletClicked()
        {
            // Get the player's wallet address to mint the fox to
            string address = await GetWalletAddress();
            // Show the player's tokens on the block explorer page.
            Application.OpenURL($"https://sandbox.immutascan.io/address/{address}?tab=1");
        }
    }
}
