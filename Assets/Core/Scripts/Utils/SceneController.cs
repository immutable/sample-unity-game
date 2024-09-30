using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace HyperCasual.Core
{
    /// <summary>
    ///     Encapsulates scene load and unload functionality respecting a NeverUnloadScene scene.
    ///     NeverUnloadScene is a scene we never unload and instantiate all level-independent managers in it.
    /// </summary>
    public class SceneController
    {
        private readonly Scene m_NeverUnloadScene;
        private Scene m_LastScene;

        /// <param name="neverUnloadScene">The scene we instantiate all level-independent managers in it and never unloads.</param>
        public SceneController(Scene neverUnloadScene)
        {
            m_NeverUnloadScene = neverUnloadScene;
            m_LastScene = m_NeverUnloadScene;
        }

        /// <summary>
        ///     Loads a scene at the given path and unload others
        /// </summary>
        /// <param name="scene">scene path</param>
        /// <exception cref="ArgumentException">scene path is invalid</exception>
        public IEnumerator LoadScene(string scene)
        {
            if (string.IsNullOrEmpty(scene))
                throw new ArgumentException($"{nameof(scene)} is invalid!");

            yield return UnloadLastScene();

            yield return LoadSceneAdditive(scene);
        }

        /// <summary>
        ///     Creates and Loads a new empty scene of the given name and unloads others.
        /// </summary>
        /// <param name="scene">scene name</param>
        /// <exception cref="ArgumentException">scene name is invalid</exception>
        public IEnumerator LoadNewScene(string scene)
        {
            if (string.IsNullOrEmpty(scene))
                throw new ArgumentException($"{nameof(scene)} is invalid!");

            yield return UnloadLastScene();

            LoadNewSceneAdditive(scene);
        }

        private IEnumerator UnloadScene(Scene scene)
        {
            if (!m_LastScene.IsValid())
                yield break;

            var asyncUnload = SceneManager.UnloadSceneAsync(scene);
            while (!asyncUnload.isDone) yield return null;
        }

        private IEnumerator LoadSceneAdditive(string scenePath)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

            while (!asyncLoad.isDone) yield return null;

            m_LastScene = SceneManager.GetSceneByPath(scenePath);
            SceneManager.SetActiveScene(m_LastScene);
        }

        private void LoadNewSceneAdditive(string sceneName)
        {
            var scene = SceneManager.CreateScene(sceneName);
            SceneManager.SetActiveScene(scene);
            m_LastScene = scene;
        }

        /// <summary>
        ///     Unloads last loaded scene
        /// </summary>
        public IEnumerator UnloadLastScene()
        {
            if (m_LastScene != m_NeverUnloadScene)
                yield return UnloadScene(m_LastScene);
        }
    }
}