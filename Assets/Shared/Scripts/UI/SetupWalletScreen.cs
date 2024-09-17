using System;
using HyperCasual.Core;
using Immutable.Passport;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     This View contains celebration screen functionalities
    /// </summary>
    public class SetupWalletScreen : View
    {
        [SerializeField] private TextMeshProUGUI m_Title;

        [SerializeField] private GameObject m_Loading;

        [SerializeField] private GameObject m_Success;

        [SerializeField] private TextMeshProUGUI m_ErrorMessage;

        [SerializeField] private HyperCasualButton m_NextButton;

        [SerializeField] private HyperCasualButton m_TryAgainButton;

        [SerializeField] private AbstractGameEvent m_MintEvent;

        [SerializeField] private AbstractGameEvent m_NextEvent;

        public void OnEnable()
        {
            // Set listener to 'Next' button
            m_NextButton.RemoveListener(OnNextButtonClicked);
            m_NextButton.AddListener(OnNextButtonClicked);

            // Set listener to "Try again" button
            m_TryAgainButton.RemoveListener(SetupWallet);
            m_TryAgainButton.AddListener(SetupWallet);

            SetupWallet();
        }

        private async void SetupWallet()
        {
            try
            {
                m_Title.text = "Setting up your wallet...";
                ShowLoading(true);
                ShowError(false);
                ShowSuccess(false);

                // Set up provider
                await Passport.Instance.ConnectEvm();
                // Set up wallet (includes creating a wallet for new players)
                var accounts = await Passport.Instance.ZkEvmRequestAccounts();
                SaveManager.Instance.WalletAddress = accounts[0]; // Grab player's first wallet

                m_Title.text = "Your wallet has been successfully set up!";
                ShowLoading(false);
                ShowError(false);
                ShowSuccess(true);
            }
            catch (Exception ex)
            {
                // Failed to set up wallet, let the player try again
                Debug.Log($"Failed to set up wallet: {ex.Message}");
                ShowLoading(false);
                ShowError(true);
                ShowSuccess(false);
            }
        }

        private void OnNextButtonClicked()
        {
            m_MintEvent.Raise();
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

        private void ShowSuccess(bool show)
        {
            m_Success.gameObject.SetActive(show);
            ShowNextButton(show);
        }
    }
}