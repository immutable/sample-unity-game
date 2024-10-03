using System;
using Cysharp.Threading.Tasks;
using HyperCasual.Core;
using Immutable.Passport;
using Immutable.Passport.Core.Logging;
using TMPro;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     This View contains main menu functionalities
    /// </summary>
    public class MainMenu : View
    {
        [SerializeField] private HyperCasualButton m_StartButton;

        [SerializeField] private AbstractGameEvent m_StartButtonEvent;

        [SerializeField] private HyperCasualButton m_InventoryButton;

        [SerializeField] private AbstractGameEvent m_InventoryButtonEvent;

        [SerializeField] private HyperCasualButton m_MarketplaceButton;

        [SerializeField] private AbstractGameEvent m_MarketplaceButtonEvent;

        [SerializeField] private HyperCasualButton m_ShopButton;

        [SerializeField] private AbstractGameEvent m_ShopButtonEvent;

        [SerializeField] private TextMeshProUGUI m_Email;

        [SerializeField] private HyperCasualButton m_LogoutButton;

        [SerializeField] private GameObject m_Loading;

        private Passport passport;

        private async void OnEnable()
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
            // Set listener to 'Shop' button
            m_ShopButton.RemoveListener(OnShopButtonClick);
            m_ShopButton.AddListener(OnShopButtonClick);

            // Initialise Passport
            Passport.LogLevel = LogLevel.Debug;
            passport = await Passport.Init(Config.CLIENT_ID, Config.ENVIRONMENT, Config.REDIRECT_URI,
                Config.LOGOUT_REIDIRECT_URI);

            // Check if the player is supposed to be logged in and if there are credentials saved
            if (SaveManager.Instance.IsLoggedIn && await Passport.Instance.HasCredentialsSaved())
            {
                // Try to log in using saved credentials
                var success = await Passport.Instance.Login(true);
                // Update the login flag
                SaveManager.Instance.IsLoggedIn = success;
                // Set up wallet if successful
                if (success)
                {
                    await Passport.Instance.ConnectEvm();
                    var accounts = await Passport.Instance.ZkEvmRequestAccounts();
                    SaveManager.Instance.WalletAddress = accounts[0];
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
            // Show the logout, inventory and marketplace button if the player is logged in
            ShowLogoutButton(SaveManager.Instance.IsLoggedIn);
            ShowInventoryButton(SaveManager.Instance.IsLoggedIn);
            ShowMarketplaceButton(SaveManager.Instance.IsLoggedIn);
            ShowShopButton(SaveManager.Instance.IsLoggedIn);
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

        private void OnStartButtonClick()
        {
            m_StartButtonEvent.Raise();
            AudioManager.Instance.PlayEffect(SoundID.ButtonSound);
        }

        private async void OnLogoutButtonClick()
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
                SaveManager.Instance.WalletAddress = null;
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

        private void ShowLoading(bool show)
        {
            m_Loading.gameObject.SetActive(show);
            ShowStartButton(!show);
        }

        private void ShowStartButton(bool show)
        {
            m_StartButton.gameObject.SetActive(show);
        }

        private void ShowLogoutButton(bool show)
        {
            m_LogoutButton.gameObject.SetActive(show);
        }

        private void ShowInventoryButton(bool show)
        {
            m_InventoryButton.gameObject.SetActive(show);
        }

        private void ShowMarketplaceButton(bool show)
        {
            m_MarketplaceButton.gameObject.SetActive(show);
        }

        private void ShowShopButton(bool show)
        {
            m_ShopButton.gameObject.SetActive(show);
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

        public void OnShopButtonClick()
        {
            m_ShopButtonEvent.Raise();
            AudioManager.Instance.PlayEffect(SoundID.ButtonSound);
        }
    }
}