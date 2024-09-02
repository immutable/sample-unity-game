using System;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Immutable.Passport;
using Cysharp.Threading.Tasks;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains main menu functionalities
    /// </summary>
    public class MainMenu : View
    {
        [SerializeField]
        HyperCasualButton m_StartButton;
        [SerializeField]
        AbstractGameEvent m_StartButtonEvent;
        [SerializeField]
        HyperCasualButton m_InventoryButton;
        [SerializeField]
        AbstractGameEvent m_InventoryButtonEvent;
        [SerializeField]
        HyperCasualButton m_MarketplaceButton;
        [SerializeField]
        AbstractGameEvent m_MarketplaceButtonEvent;
        [SerializeField]
        TextMeshProUGUI m_Email;
        [SerializeField]
        HyperCasualButton m_LogoutButton;

        [SerializeField]
        GameObject m_Loading;

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
            // Set listener to 'Inventory' button
            m_InventoryButton.RemoveListener(OnInventoryButtonClick);
            m_InventoryButton.AddListener(OnInventoryButtonClick);
            // Set listener to 'Marketplace' button
            m_MarketplaceButton.RemoveListener(OnMarketplaceButtonClick);
            m_MarketplaceButton.AddListener(OnMarketplaceButtonClick);

            // Initialise Passport
            string clientId = "ZJL7JvetcDFBNDlgRs5oJoxuAUUl6uQj";
            string environment = Immutable.Passport.Model.Environment.SANDBOX;
            string redirectUri = null;
            string logoutUri = null;

#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
            redirectUri = "immutablerunner://callback";
            logoutUri = "immutablerunner://logout";
#endif
            passport = await Passport.Init(clientId, environment, redirectUri, logoutUri);

            // Check if the player is supposed to be logged in and if there are credentials saved
            if (SaveManager.Instance.IsLoggedIn && await Passport.Instance.HasCredentialsSaved())
            {
                // Try to log in using saved credentials
                bool success = await Passport.Instance.Login(useCachedSession: true);
                // Update the login flag
                SaveManager.Instance.IsLoggedIn = success;
                // Set up wallet if successful
                if (success)
                {
                    await Passport.Instance.ConnectEvm();
                    await Passport.Instance.ZkEvmRequestAccounts();
                    await GetPlayersEmail();
                }
            }
            else
            {
                // No saved credentials to re-login the player, reset the login flag
                SaveManager.Instance.IsLoggedIn = false;
            }

            ShowLoading(false);
            // Show the logout, inventory and marketplace button if the player is logged in
            ShowLogoutButton(SaveManager.Instance.IsLoggedIn);
            ShowInventoryButton(SaveManager.Instance.IsLoggedIn);
            ShowMarketplaceButton(SaveManager.Instance.IsLoggedIn);
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
                ShowInventoryButton(false);
                ShowMarketplaceButton(false);
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
                ShowInventoryButton(false);
                ShowMarketplaceButton(false);
                // Reset all other values
                SaveManager.Instance.Clear();
                m_Email.text = "";
                m_Email.gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to log out: {ex.Message}");
                // Failed to logout so show 'Logout' button again
                ShowLogoutButton(true);
                ShowInventoryButton(true);
                ShowMarketplaceButton(true);
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

        void ShowInventoryButton(bool show)
        {
            m_InventoryButton.gameObject.SetActive(show);
        }

        void ShowMarketplaceButton(bool show)
        {
            m_MarketplaceButton.gameObject.SetActive(show);
        }

        void ShowEmail(bool show)
        {
            m_Email.gameObject.SetActive(show);
        }

        public void OnInventoryButtonClick()
        {
            m_InventoryButtonEvent.Raise();
            AudioManager.Instance.PlayEffect(SoundID.ButtonSound);
        }

        public void OnMarketplaceButtonClick()
        {
            m_MarketplaceButtonEvent.Raise();
            AudioManager.Instance.PlayEffect(SoundID.ButtonSound);
        }
    }
}