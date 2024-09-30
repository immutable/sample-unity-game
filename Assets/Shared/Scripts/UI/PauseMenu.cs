using HyperCasual.Core;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     This View contains pause menu functionalities
    /// </summary>
    public class PauseMenu : View
    {
        [SerializeField] private HyperCasualButton m_ContinueButton;

        [SerializeField] private HyperCasualButton m_QuitButton;

        [SerializeField] private AbstractGameEvent m_ContinueEvent;

        [SerializeField] private AbstractGameEvent m_QuitEvent;

        private void OnEnable()
        {
            m_ContinueButton.AddListener(OnContinueClicked);
            m_QuitButton.AddListener(OnQuitClicked);
        }

        private void OnDisable()
        {
            m_ContinueButton.RemoveListener(OnContinueClicked);
            m_QuitButton.RemoveListener(OnQuitClicked);
        }

        private void OnContinueClicked()
        {
            m_ContinueEvent.Raise();
        }

        private void OnQuitClicked()
        {
            m_QuitEvent.Raise();
        }
    }
}