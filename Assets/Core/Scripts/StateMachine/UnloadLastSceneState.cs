using System.Collections;

namespace HyperCasual.Core
{
    /// <summary>
    ///     Unloads a currently loaded scene
    /// </summary>
    public class UnloadLastSceneState : AbstractState
    {
        private readonly SceneController m_SceneController;

        /// <param name="sceneController">The SceneController for the current unloading operation</param>
        public UnloadLastSceneState(SceneController sceneController)
        {
            m_SceneController = sceneController;
        }

        public override IEnumerator Execute()
        {
            yield return m_SceneController.UnloadLastScene();
        }
    }
}