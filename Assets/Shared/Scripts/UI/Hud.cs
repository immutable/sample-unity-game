using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using HyperCasual.Runner;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Gameplay
{
    /// <summary>
    /// This View contains head-up-display functionalities
    /// </summary>
    public class Hud : View
    {
        [SerializeField]
        TextMeshProUGUI m_FoodText;
        [SerializeField]
        Slider m_XpSlider;
        [SerializeField]
        HyperCasualButton m_PauseButton;
        [SerializeField]
        AbstractGameEvent m_PauseEvent;

        /// <summary>
        /// The slider that displays the XP value 
        /// </summary>
        public Slider XpSlider => m_XpSlider;

        int m_FoodValue;

        /// <summary>
        /// The amount of gold to display on the hud.
        /// The setter method also sets the hud text.
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
        /// The amount of XP to display on the hud.
        /// The setter method also sets the hud slider value.
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

        void OnEnable()
        {
            m_PauseButton.AddListener(OnPauseButtonClick);
        }

        void OnDisable()
        {
            m_PauseButton.RemoveListener(OnPauseButtonClick);
        }

        void OnPauseButtonClick()
        {
            m_PauseEvent.Raise();
        }
    }
}
