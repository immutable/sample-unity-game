using System.ComponentModel;
using HyperCasual.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains celebration screen functionalities
    /// </summary>
    public class LevelCompleteScreen : View
    {
        // Completed screen
        [SerializeField]
        HyperCasualButton m_NextButton;
        [SerializeField]
        HyperCasualButton m_ContinuePassportButton;
        [SerializeField]
        Image[] m_Coins;
        [SerializeField]
        AbstractGameEvent m_NextLevelEvent;
        [SerializeField]
        TextMeshProUGUI m_FoodText;
        [SerializeField]
        Slider m_XpSlider;
        [SerializeField]
        GameObject m_Loading;
        [SerializeField]
        GameObject m_CompletedContainer;

        // Minted screen
        [SerializeField]
        GameObject m_MintedContainer;
        [SerializeField]
        TextMeshProUGUI m_MintedTitle;
        [SerializeField]
        HyperCasualButton m_MintedNextButton;

        // Unlocked Skin screen
        [SerializeField]
        GameObject m_SkinUnlockedContainer;
        [SerializeField]
        TextMeshProUGUI m_SkinUnlockedErrorMessage;
        [SerializeField]
        HyperCasualButton m_SkinUnlockedGetButton;
        [SerializeField]
        HyperCasualButton m_SkinUnlockedNextButton;
        [SerializeField]
        GameObject m_SkinUnlockedLoading;

        // Collected Skin screen
        [SerializeField]
        GameObject m_CollectedSkinContainer;
        [SerializeField]
        TextMeshProUGUI m_CollectedSkinErrorMessage;
        [SerializeField]
        HyperCasualButton m_CollectedSkinUseButton;
        [SerializeField]
        HyperCasualButton m_CollectedSkinNextButton;
        [SerializeField]
        GameObject m_CollectedSkinLoading;
        
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

        public void OnEnable()
        {
            ShowNextButton(true);
            m_NextButton.RemoveListener(OnNextButtonClicked);
            m_NextButton.AddListener(OnNextButtonClicked);
        }

        void OnNextButtonClicked()
        {
            ShowCompletedContainer(true);
            m_NextLevelEvent.Raise();
        }

        // Level complete
        void ShowCompletedContainer(bool show)
        {
            m_CompletedContainer.gameObject.SetActive(show);
        }

        void ShowContinueWithPassportButton(bool show)
        {
            m_ContinuePassportButton.gameObject.SetActive(show);
        }

        void ShowNextButton(bool show)
        {
            m_NextButton.gameObject.SetActive(show);
        }

        void ShowLoading(bool show)
        {
            m_Loading.gameObject.SetActive(show);
        }

        // Minted screen
        void ShowMintedContainer(bool show)
        {
            m_MintedContainer.gameObject.SetActive(show);
        }

        // Unlocked Skin screen
        void ShowSkinUnlockedContainer(bool show)
        {
            m_SkinUnlockedContainer.gameObject.SetActive(show);
        }

        // Collected Skin screen
        void ShowCollectedSkinContainer(bool show)
        {
            m_CollectedSkinContainer.gameObject.SetActive(show);
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
