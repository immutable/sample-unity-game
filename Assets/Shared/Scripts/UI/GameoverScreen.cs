using HyperCasual.Core;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     This View contains Game-over Screen functionalities
    /// </summary>
    public class GameoverScreen : View
    {
        [SerializeField] private HyperCasualButton m_PlayAgainButton;

        [SerializeField] private HyperCasualButton m_GoToMainMenuButton;

        [SerializeField] private AbstractGameEvent m_PlayAgainEvent;

        [SerializeField] private AbstractGameEvent m_GoToMainMenuEvent;

        private void OnEnable()
        {
            m_PlayAgainButton.AddListener(OnPlayAgainButtonClick);
            m_GoToMainMenuButton.AddListener(OnGoToMainMenuButtonClick);
        }

        private void OnDisable()
        {
            m_PlayAgainButton.RemoveListener(OnPlayAgainButtonClick);
            m_GoToMainMenuButton.RemoveListener(OnGoToMainMenuButtonClick);
        }

        private void OnPlayAgainButtonClick()
        {
            m_PlayAgainEvent.Raise();
        }

        private void OnGoToMainMenuButtonClick()
        {
            m_GoToMainMenuEvent.Raise();
        }
    }
}