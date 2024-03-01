using System;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        void OnEnable()
        {
            ShowLoading(true);

            // Set listener to 'Start' button
            m_StartButton.RemoveListener(OnStartButtonClick);
            m_StartButton.AddListener(OnStartButtonClick);
            // Set listener to 'Logout' button
            m_LogoutButton.RemoveListener(OnLogoutButtonClick);
            m_LogoutButton.AddListener(OnLogoutButtonClick);
            
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
        }

        void ShowLoading(bool show)
        {
            m_Loading.gameObject.SetActive(show);
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