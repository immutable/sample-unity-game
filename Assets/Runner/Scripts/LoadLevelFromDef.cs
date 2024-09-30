using System;
using System.Collections;
using HyperCasual.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HyperCasual.Runner
{
    public class LoadLevelFromDef : AbstractState
    {
        public readonly LevelDefinition m_LevelDefinition;
        private readonly GameObject[] m_ManagerPrefabs;
        private readonly SceneController m_SceneController;

        public LoadLevelFromDef(SceneController sceneController, AbstractLevelData levelData,
            GameObject[] managerPrefabs)
        {
            if (levelData is LevelDefinition levelDefinition)
                m_LevelDefinition = levelDefinition;

            m_ManagerPrefabs = managerPrefabs;
            m_SceneController = sceneController;
        }

        public override IEnumerator Execute()
        {
            if (m_LevelDefinition == null)
                throw new Exception($"{nameof(m_LevelDefinition)} is null!");

            yield return m_SceneController.LoadNewScene(nameof(m_LevelDefinition));

            // Load managers specific to the level
            foreach (var prefab in m_ManagerPrefabs) Object.Instantiate(prefab);

            GameManager.Instance.LoadLevel(m_LevelDefinition);
        }
    }
}