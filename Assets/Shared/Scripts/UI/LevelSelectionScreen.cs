using System;
using System.Collections.Generic;
using HyperCasual.Core;
using HyperCasual.Runner;
using UnityEngine;

namespace HyperCasual.Gameplay
{
    /// <summary>
    ///     This View contains level selection screen functionalities
    /// </summary>
    public class LevelSelectionScreen : View
    {
        [SerializeField] private HyperCasualButton m_QuickPlayButton;

        [SerializeField] private HyperCasualButton m_BackButton;

        [Space][SerializeField] private LevelSelectButton m_LevelButtonPrefab;

        [SerializeField] private RectTransform m_LevelButtonsRoot;

        [SerializeField] private AbstractGameEvent m_NextLevelEvent;

        [SerializeField] private AbstractGameEvent m_BackEvent;
#if UNITY_EDITOR
        [SerializeField] private bool m_UnlockAllLevels;
#endif

        private readonly List<LevelSelectButton> m_Buttons = new();

        private void Start()
        {
            var levels = SequenceManager.Instance.Levels;
            var levelProgress = SaveManager.Instance.LevelProgress;
            for (var i = 0; i < levels.Length; i++) m_Buttons.Add(Instantiate(m_LevelButtonPrefab, m_LevelButtonsRoot));

            ResetButtonData();
        }

        private void OnEnable()
        {
            ResetButtonData();

            m_QuickPlayButton.AddListener(OnQuickPlayButtonClicked);
            m_BackButton.AddListener(OnBackButtonClicked);
        }

        private void OnDisable()
        {
            m_QuickPlayButton.RemoveListener(OnQuickPlayButtonClicked);
            m_BackButton.RemoveListener(OnBackButtonClicked);
        }

        private void ResetButtonData()
        {
            var levelProgress = SaveManager.Instance.LevelProgress;
            for (var i = 0; i < m_Buttons.Count; i++)
            {
                var button = m_Buttons[i];
                var unlocked = i <= levelProgress;
#if UNITY_EDITOR
                unlocked = unlocked || m_UnlockAllLevels;
#endif
                button.SetData(i, unlocked, OnClick);
            }
        }

        private void OnClick(int startingIndex)
        {
            if (startingIndex < 0)
                throw new Exception("Button is not initialized");

            SequenceManager.Instance.SetStartingLevel(startingIndex);
            m_NextLevelEvent.Raise();
        }

        private void OnQuickPlayButtonClicked()
        {
            OnClick(SaveManager.Instance.LevelProgress);
        }

        private void OnBackButtonClicked()
        {
            m_BackEvent.Raise();
        }
    }
}