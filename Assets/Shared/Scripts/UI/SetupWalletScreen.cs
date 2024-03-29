using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using Immutable.Passport;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains celebration screen functionalities
    /// </summary>
    public class SetupWalletScreen : View
    {

        [SerializeField]
        TextMeshProUGUI m_Title;
        [SerializeField]
        GameObject m_Loading;
        [SerializeField]
        GameObject m_Success;
        [SerializeField]
        TextMeshProUGUI m_ErrorMessage;
        [SerializeField]
        HyperCasualButton m_NextButton;
        [SerializeField]
        HyperCasualButton m_TryAgainButton;
        [SerializeField]
        AbstractGameEvent m_MintEvent;
        [SerializeField]
        AbstractGameEvent m_NextEvent;

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
                await Passport.Instance.ZkEvmRequestAccounts();

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
