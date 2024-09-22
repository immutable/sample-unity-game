using System;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Immutable.Passport;
using Immutable.Passport.Core.Logging;
using Cysharp.Threading.Tasks;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains main menu functionalities
    /// </summary>
    public class MainMenu : View
    {
        [SerializeField] HyperCasualButton m_StartButton;
        [SerializeField] AbstractGameEvent m_StartButtonEvent;
        [SerializeField] TextMeshProUGUI m_Email;
        [SerializeField] HyperCasualButton m_LogoutButton;
        [SerializeField] GameObject m_Loading;
        [SerializeField] Toggle m_ZkEVMToggle;

        Passport passport;

        async void OnEnable()
        {
            ShowLoading(true);
            m_Email.gameObject.SetActive(false);

            // Set listener to 'Start' button
            m_StartButton.RemoveListener(OnStartButtonClick);
            m_StartButton.AddListener(OnStartButtonClick);
            // Set listener to 'Logout' button
            m_LogoutButton.RemoveListener(OnLogoutButtonClick);
            m_LogoutButton.AddListener(OnLogoutButtonClick);

            // Set Passport log level
            Passport.LogLevel = LogLevel.Info;

            // Initialise Passport
            string environment = Immutable.Passport.Model.Environment.SANDBOX;
            passport = await Passport.Init(Config.CLIENT_ID, environment, Config.REDIRECT_URI, Config.LOGOUT_URI);

            // Check which chain to use
            m_ZkEVMToggle.isOn = SaveManager.Instance.ZkEvm;
            m_ZkEVMToggle.onValueChanged.AddListener(delegate
            {
                SaveManager.Instance.ZkEvm = m_ZkEVMToggle.isOn;
                SetupRollup();
            });

            // Initial set up
            SetupRollup();
        }

        private async void SetupRollup()
        {
            ShowLoading(true);

            // Check if the player is supposed to be logged in and if there are credentials saved
            if (SaveManager.Instance.IsLoggedIn && await Passport.Instance.HasCredentialsSaved())
            {
                // Try to log in using saved credentials
                bool success;
                if (SaveManager.Instance.ZkEvm)
                {
                    success = await Passport.Instance.Login(useCachedSession: true);
                    if (success)
                    {
                        await Passport.Instance.ConnectEvm();
                        List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
                        SaveManager.Instance.WalletAddress = accounts[0];
                    }
                }
                else
                {
                    success = await Passport.Instance.ConnectImx(useCachedSession: true);
                    if (success)
                    {
                        SaveManager.Instance.WalletAddress = await Passport.Instance.GetAddress();
                    }
                }

                // Update the login flag and retrieve player's email if login is successful
                SaveManager.Instance.IsLoggedIn = success;
                if (success)
                {
                    await GetPlayersEmail();
                }
            }
            else
            {
                // No saved credentials to re-login the player, reset the login flag
                SaveManager.Instance.IsLoggedIn = false;
                SaveManager.Instance.WalletAddress = null;
            }

            ShowLoading(false);
            // Show the logout button if the player is logged in
            ShowLogoutButton(SaveManager.Instance.IsLoggedIn);

        }

        void OnDisable()
        {
            m_StartButton.RemoveListener(OnStartButtonClick);
        }

        private async UniTask GetPlayersEmail()
        {
            try
            {
                m_Email.text = await Passport.Instance.GetEmail();
                m_Email.gameObject.SetActive(true);
            }
            catch (Exception ex)
            {
                Debug.Log("Failed to get player's email");
                m_Email.gameObject.SetActive(false);
            }
        }

        void OnStartButtonClick()
        {
            m_StartButtonEvent.Raise();
            AudioManager.Instance.PlayEffect(SoundID.ButtonSound);
        }

        async void OnLogoutButtonClick()
        {
            try
            {
                // Hide the 'Logout' button
                ShowLogoutButton(false);
                // Show loading
                ShowLoading(true);

                // Logout
#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
                await passport.LogoutPKCE();
#else
                await passport.Logout();
#endif

                // Reset the login flag
                SaveManager.Instance.IsLoggedIn = false;
                // Successfully logged out, hide 'Logout' button
                ShowLogoutButton(false);

                // Save rollup selection
                bool zkEVM = SaveManager.Instance.ZkEvm;

                // Reset all other values
                SaveManager.Instance.Clear();
                m_Email.text = "";
                m_Email.gameObject.SetActive(false);

                // Restore rollup selection
                SaveManager.Instance.ZkEvm = zkEVM;
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to log out: {ex.Message}");
                // Failed to logout so show 'Logout' button again
                ShowLogoutButton(true);
            }
            // Hide loading
            ShowLoading(false);
        }

        void ShowLoading(bool show)
        {
            m_Loading.gameObject.SetActive(show);
            ShowStartButton(!show);
        }

        void ShowStartButton(bool show)
        {
            m_StartButton.gameObject.SetActive(show);
        }

        void ShowLogoutButton(bool show)
        {
            m_LogoutButton.gameObject.SetActive(show);
        }
    }
}