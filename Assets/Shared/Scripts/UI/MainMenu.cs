using System;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Immutable.Passport;

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
        TextMeshProUGUI m_Email;
        [SerializeField]
        HyperCasualButton m_LogoutButton;
        [SerializeField]
        GameObject m_Loading;

        Passport passport;

        async void OnEnable()
        {
            ShowLoading(true);

            // Set listener to 'Start' button
            m_StartButton.RemoveListener(OnStartButtonClick);
            m_StartButton.AddListener(OnStartButtonClick);
            // Set listener to 'Logout' button
            m_LogoutButton.RemoveListener(OnLogoutButtonClick);
            m_LogoutButton.AddListener(OnLogoutButtonClick);

            // Initialise Passport
            string clientId = "ZJL7JvetcDFBNDlgRs5oJoxuAUUl6uQj";
            string environment = Immutable.Passport.Model.Environment.SANDBOX;
            string redirectUri = null;
            string logoutRedirectUri = null;

#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
            redirectUri = "immutablerunner://callback";
            logoutRedirectUri = "immutablerunner://logout";
#endif
            passport = await Passport.Init(clientId, environment, redirectUri, logoutRedirectUri);

            // Check if the player is supposed to be logged in
            if (SaveManager.Instance.IsLoggedIn)
            {
                // Check if there are credentials saved
                bool hasCredentialsSaved = await Passport.Instance.HasCredentialsSaved();
                if (hasCredentialsSaved)
                {
                    // Try to log in using saved credentials
                    bool success = await Passport.Instance.Login(useCachedSession: true);
                    // Update the login flag
                    SaveManager.Instance.IsLoggedIn = success;
                }
                else
                {
                    // No saved credentials to re-login the player, reset the login flag
                    SaveManager.Instance.IsLoggedIn = false;
                }
            }

            ShowLoading(false);
            ShowStartButton(true);
        }

        void OnDisable()
        {
            m_StartButton.RemoveListener(OnStartButtonClick);
        }

        void OnStartButtonClick()
        {
            m_StartButtonEvent.Raise();
            AudioManager.Instance.PlayEffect(SoundID.ButtonSound);
        }

        void OnLogoutButtonClick()
        {
            try
            {
                // Hide the 'Logout' button
                ShowLogoutButton(false);
                // Show loading
                ShowLoading(true);

                // Logout

                // Reset the login flag
                SaveManager.Instance.IsLoggedIn = false;
                // Successfully logged out, hide 'Logout' button
                ShowLogoutButton(false);
                // Reset all other values
                SaveManager.Instance.Clear();
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

        void ShowEmail(bool show)
        {
            m_Email.gameObject.SetActive(show);
        }
    }
}