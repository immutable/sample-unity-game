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
    public class CollectedSkinScreen : View
    {
        [SerializeField]
        HyperCasualButton m_UseButton;
        [SerializeField]
        HyperCasualButton m_NextButton;
        [SerializeField]
        AbstractGameEvent m_NextLevelEvent;

        public void OnEnable()
        {
            // Set listener to 'Next' button
            m_NextButton.RemoveListener(OnNextButtonClicked);
            m_NextButton.AddListener(OnNextButtonClicked);

            // Set listener to 'Use New Skin' button
            m_UseButton.RemoveListener(OnUseButtonClicked);
            m_UseButton.AddListener(OnUseButtonClicked);
        }

        private void OnUseButtonClicked()
        {
            // For the purpose of this tutorial, we will save the flag to use the new skin in memory only
            // (i.e. if you start the game again, this flag will reset)
            MemoryCache.UseNewSkin = true;
            m_NextLevelEvent.Raise();
        }

        private void OnNextButtonClicked()
        {
            m_NextLevelEvent.Raise();
        }
    }
}
