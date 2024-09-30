using HyperCasual.Core;
using HyperCasual.Runner;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Gameplay
{
    /// <summary>
    ///     This View contains head-up-display functionalities
    /// </summary>
    public class Hud : View
    {
        [SerializeField] private TextMeshProUGUI m_FoodText;

        [SerializeField] private Slider m_XpSlider;

        [SerializeField] private HyperCasualButton m_PauseButton;

        [SerializeField] private AbstractGameEvent m_PauseEvent;

        private int m_FoodValue;

        private float m_XpValue;

        /// <summary>
        ///     The slider that displays the XP value
        /// </summary>
        public Slider XpSlider => m_XpSlider;

        /// <summary>
        ///     The amount of gold to display on the hud.
        ///     The setter method also sets the hud text.
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

        /// <summary>
        ///     The amount of XP to display on the hud.
        ///     The setter method also sets the hud slider value.
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

        private void OnEnable()
        {
            m_PauseButton.AddListener(OnPauseButtonClick);
        }

        private void OnDisable()
        {
            m_PauseButton.RemoveListener(OnPauseButtonClick);
        }

        private void OnPauseButtonClick()
        {
            m_PauseEvent.Raise();
        }
    }
}