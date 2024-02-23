using System.ComponentModel;
using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Immutable.Passport;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains celebration screen functionalities
    /// </summary>
    public class LevelCompleteScreen : View
    {
        [SerializeField]
        HyperCasualButton m_NextButton;
        [SerializeField]
        HyperCasualButton m_TryAgainButton;
        [SerializeField]
        HyperCasualButton m_ContinuePassportButton;
        [SerializeField]
        Image[] m_Coins;
        [SerializeField]
        TextMeshProUGUI m_FoodText;
        [SerializeField]
        Slider m_XpSlider;
        [SerializeField]
        GameObject m_Loading;
        [SerializeField]
        GameObject m_CompletedContainer;
        [SerializeField]
        TextMeshProUGUI m_ErrorMessage;

        [SerializeField]
        AbstractGameEvent m_NextLevelEvent;
        [SerializeField]
        AbstractGameEvent m_SetupWalletEvent;
        [SerializeField]
        AbstractGameEvent m_UnlockedSkinEvent;

        /// <summary>
        /// The slider that displays the XP value 
        /// </summary>
        public Slider XpSlider => m_XpSlider;

        int m_FoodValue;

        private ApiService Api = new();

        /// <summary>
        /// The amount of food to display on the celebration screen.
        /// The setter method also sets the celebration screen text.
        /// </summary>
        public int FoodValue
        {
            get => m_FoodValue;
            set
            {
                if (m_FoodValue != value)
                {
                    m_FoodValue = value;
                    m_FoodText.text = FoodValue.ToString();
                }
            }
        }

        float m_XpValue;

        /// <summary>
        /// The amount of XP to display on the celebration screen.
        /// The setter method also sets the celebration screen slider value.
        /// </summary>
        public float XpValue
        {
            get => m_XpValue;
            set
            {
                if (!Mathf.Approximately(m_XpValue, value))
                {
                    m_XpValue = value;
                    m_XpSlider.value = m_XpValue;
                }
            }
        }

        int m_CoinCount = -1;

        /// <summary>
        /// The number of tokens to display on the celebration screen.
        /// </summary>
        public int CoinCount
        {
            get => m_CoinCount;
            set
            {
                if (m_CoinCount != value)
                {
                    m_CoinCount = value;
                    DisplayCoins(m_CoinCount);
                }
            }
        }

        public async void OnEnable()
        {
            // Set listener to 'Next' button
            m_NextButton.RemoveListener(OnNextButtonClicked);
            m_NextButton.AddListener(OnNextButtonClicked);

            // Set listener to "Continue with Passport" button
            m_ContinuePassportButton.RemoveListener(OnContinueWithPassportButtonClicked);
            m_ContinuePassportButton.AddListener(OnContinueWithPassportButtonClicked);

            // Set listener to "Try again" button
            m_TryAgainButton.RemoveListener(OnTryAgainButtonClicked);
            m_TryAgainButton.AddListener(OnTryAgainButtonClicked);

            // Show 'Next' button if player is already logged into Passport
            ShowNextButton(SaveManager.Instance.IsLoggedIn);
            // Show "Continue with Passport" button if the player is not logged into Passport
            ShowContinueWithPassportButton(!SaveManager.Instance.IsLoggedIn);
        }

        private async void OnContinueWithPassportButtonClicked()
        {
            try
            {
                // Show loading
                ShowContinueWithPassportButton(false);
                ShowLoading(true);

                // Log into Passport
#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
                await Passport.Instance.LoginPKCE();
#else
                await Passport.Instance.Login();
#endif

                // Successfully logged in
                // Save a persistent flag in the game that the player is logged in
                SaveManager.Instance.IsLoggedIn = true;
                // Show 'Next' button
                ShowNextButton(true);
                ShowLoading(false);
                // Take the player to the Setup Wallet screen
                m_SetupWalletEvent.Raise();
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to log into Passport: {ex.Message}");
                // Show Continue with Passport button again
                ShowContinueWithPassportButton(true);
                ShowLoading(false);
            }
        }

        private void OnTryAgainButtonClicked()
        {
        }

        private void OnNextButtonClicked()
        {
            m_NextLevelEvent.Raise();
        }

        private void ShowCompletedContainer(bool show)
        {
            m_CompletedContainer.gameObject.SetActive(show);
        }

        private void ShowContinueWithPassportButton(bool show)
        {
            m_ContinuePassportButton.gameObject.SetActive(show);
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

        void DisplayCoins(int count)
        {
            count = Mathf.Clamp(count, 0, m_Coins.Length);

            if (m_Coins.Length > 0 && count >= 0 && count <= m_Coins.Length)
            {
                for (int i = 0; i < m_Coins.Length; i++)
                {
                    m_Coins[i].gameObject.SetActive(i < count);
                }
            }
        }
    }
}