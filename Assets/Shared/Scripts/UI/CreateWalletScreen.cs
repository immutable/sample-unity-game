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
    public class CreateWalletScreen : View
    {
        
        [SerializeField]
        TextMeshProUGUI m_Title;
        [SerializeField]
        GameObject m_Loading;
        [SerializeField]
        TextMeshProUGUI m_ErrorMessage;
        [SerializeField]
        HyperCasualButton m_NextButton;
        [SerializeField]
        HyperCasualButton m_TryAgainButton;
        [SerializeField]
        AbstractGameEvent m_MintEvent;

        public void OnEnable()
        {
            // Set listener to 'Next' button
            m_NextButton.RemoveListener(OnNextButtonClicked);
            m_NextButton.AddListener(OnNextButtonClicked);

            // Set listener to "Try again" button
            m_TryAgainButton.RemoveListener(CreateWallet);
            m_TryAgainButton.AddListener(CreateWallet);

            CreateWallet();
        }

        private void CreateWallet()
        {
            try
            {
                m_Title.text = "Creating your wallet...";
                ShowLoading(true);
                ShowError(false);
                ShowNextButton(false);

                // Create wallet

                m_Title.text = "Created your wallet!";
                ShowLoading(false);
                ShowError(false);
                ShowNextButton(true);
            }
            catch (Exception ex)
            {
                // Failed to create wallet, let the player try again
                Debug.Log($"Failed to create wallet: {ex.Message}");
                ShowLoading(false);
                ShowError(true);
                ShowNextButton(false);
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
    }
}
