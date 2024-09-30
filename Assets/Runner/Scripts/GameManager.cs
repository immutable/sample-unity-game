using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HyperCasual.Runner
{
    /// <summary>
    ///     A class used to store game state information,
    ///     load levels, and save/load statistics as applicable.
    ///     The GameManager class manages all game-related
    ///     state changes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AbstractGameEvent m_WinEvent;

        [SerializeField] private AbstractGameEvent m_LoseEvent;

        private List<Spawnable> m_ActiveSpawnables = new();

        private LevelDefinition m_CurrentLevel;
        private GameObject m_CurrentLevelGO;
        private GameObject m_CurrentTerrainGO;

#if UNITY_EDITOR
        private bool m_LevelEditorMode;
#endif
        private GameObject m_LevelMarkersGO;

        /// <summary>
        ///     Returns the GameManager.
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        ///     Returns true if the game is currently active.
        ///     Returns false if the game is paused, has not yet begun,
        ///     or has ended.
        /// </summary>
        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

#if UNITY_EDITOR
            // If LevelManager already exists, user is in the LevelEditorWindow
            if (LevelManager.Instance != null)
            {
                StartGame();
                m_LevelEditorMode = true;
            }
#endif
        }

        /// <summary>
        ///     This method calls all methods necessary to load and
        ///     instantiate a level from a level definition.
        /// </summary>
        public void LoadLevel(LevelDefinition levelDefinition)
        {
            m_CurrentLevel = levelDefinition;
            LoadLevel(m_CurrentLevel, ref m_CurrentLevelGO);
            CreateTerrain(m_CurrentLevel, ref m_CurrentTerrainGO);
            PlaceLevelMarkers(m_CurrentLevel, ref m_LevelMarkersGO);
            SetSkyBox();
            StartGame();
        }

        /// <summary>
        ///     This method calls all methods necessary to restart a level,
        ///     including resetting the player to their starting position
        /// </summary>
        public void ResetLevel()
        {
            if (PlayerController.Instance != null) PlayerController.Instance.ResetPlayer();

            if (CameraManager.Instance != null) CameraManager.Instance.ResetCamera();

            if (LevelManager.Instance != null) LevelManager.Instance.ResetSpawnables();
        }

        /// <summary>
        ///     This method loads and instantiates the level defined in levelDefinition,
        ///     storing a reference to its parent GameObject in levelGameObject
        /// </summary>
        /// <param name="levelDefinition">
        ///     A LevelDefinition ScriptableObject that holds all information needed to
        ///     load and instantiate a level.
        /// </param>
        /// <param name="levelGameObject">
        ///     A new GameObject to be created, acting as the parent for the level to be loaded
        /// </param>
        public static void LoadLevel(LevelDefinition levelDefinition, ref GameObject levelGameObject)
        {
            if (levelDefinition == null)
            {
                Debug.LogError("Invalid Level!");
                return;
            }

            if (levelGameObject != null)
            {
                if (Application.isPlaying)
                    Destroy(levelGameObject);
                else
                    DestroyImmediate(levelGameObject);
            }

            levelGameObject = new GameObject("LevelManager");
            var levelManager = levelGameObject.AddComponent<LevelManager>();
            levelManager.LevelDefinition = levelDefinition;

            var levelParent = levelGameObject.transform;

            for (var i = 0; i < levelDefinition.Spawnables.Length; i++)
            {
                var spawnableObject = levelDefinition.Spawnables[i];

                if (spawnableObject.SpawnablePrefab == null) continue;

                var position = spawnableObject.Position;
                var eulerAngles = spawnableObject.EulerAngles;
                var scale = spawnableObject.Scale;

                GameObject go = null;

                if (Application.isPlaying)
                {
                    go = Instantiate(spawnableObject.SpawnablePrefab, position, Quaternion.Euler(eulerAngles));
                }
                else
                {
#if UNITY_EDITOR
                    go = (GameObject)PrefabUtility.InstantiatePrefab(spawnableObject.SpawnablePrefab);
                    go.transform.position = position;
                    go.transform.eulerAngles = eulerAngles;
#endif
                }

                if (go == null) return;

                // Set Base Color
                var spawnable = go.GetComponent<Spawnable>();
                if (spawnable != null)
                    // spawnable.SetBaseColor(spawnableObject.BaseColor);
                    // spawnable.SetScale(scale);
                    levelManager.AddSpawnable(spawnable);

                if (go != null) go.transform.SetParent(levelParent);
            }
        }

        public void UnloadCurrentLevel()
        {
            if (m_CurrentLevelGO != null) Destroy(m_CurrentLevelGO);

            if (m_LevelMarkersGO != null) Destroy(m_LevelMarkersGO);

            if (m_CurrentTerrainGO != null) Destroy(m_CurrentTerrainGO);

            m_CurrentLevel = null;
        }

        private void StartGame()
        {
            ResetLevel();
            IsPlaying = true;
        }

        /// <summary>
        ///     Creates and instantiates the StartPrefab and EndPrefab defined inside
        ///     the levelDefinition.
        /// </summary>
        /// <param name="levelDefinition">
        ///     A LevelDefinition ScriptableObject that defines the start and end prefabs.
        /// </param>
        /// <param name="levelMarkersGameObject">
        ///     A new GameObject that is created to be the parent of the start and end prefabs.
        /// </param>
        public static void PlaceLevelMarkers(LevelDefinition levelDefinition, ref GameObject levelMarkersGameObject)
        {
            if (levelMarkersGameObject != null)
            {
                if (Application.isPlaying)
                    Destroy(levelMarkersGameObject);
                else
                    DestroyImmediate(levelMarkersGameObject);
            }

            levelMarkersGameObject = new GameObject("Level Markers");

            var start = levelDefinition.StartPrefab;
            var end = levelDefinition.EndPrefab;

            if (start != null)
            {
                var go = Instantiate(start, new Vector3(start.transform.position.x, start.transform.position.y, 0.0f),
                    Quaternion.identity);
                go.transform.SetParent(levelMarkersGameObject.transform);
            }

            if (end != null)
            {
                var go = Instantiate(end,
                    new Vector3(end.transform.position.x, end.transform.position.y, levelDefinition.LevelLength),
                    Quaternion.identity);
                go.transform.SetParent(levelMarkersGameObject.transform);
            }
        }

        /// <summary>
        ///     Creates and instantiates a Terrain GameObject, built
        ///     to the specifications saved in levelDefinition.
        /// </summary>
        /// <param name="levelDefinition">
        ///     A LevelDefinition ScriptableObject that defines the terrain size.
        /// </param>
        /// <param name="terrainGameObject">
        ///     A new GameObject that is created to hold the terrain.
        /// </param>
        public static void CreateTerrain(LevelDefinition levelDefinition, ref GameObject terrainGameObject)
        {
            var terrainDimensions = new TerrainGenerator.TerrainDimensions
            {
                Width = levelDefinition.LevelWidth,
                Length = levelDefinition.LevelLength,
                StartBuffer = levelDefinition.LevelLengthBufferStart,
                EndBuffer = levelDefinition.LevelLengthBufferEnd,
                Thickness = levelDefinition.LevelThickness
            };
            TerrainGenerator.CreateTerrain(terrainDimensions, levelDefinition.TerrainMaterial, ref terrainGameObject);
        }

        private void SetSkyBox()
        {
            var nightSkyBox = Resources.Load("Skybox1", typeof(Material)) as Material;
            RenderSettings.skybox = nightSkyBox;
        }

        public void Win()
        {
            m_WinEvent.Raise();

#if UNITY_EDITOR
            if (m_LevelEditorMode) ResetLevel();
#endif
        }

        public void Lose()
        {
            m_LoseEvent.Raise();

#if UNITY_EDITOR
            if (m_LevelEditorMode) ResetLevel();
#endif
        }
    }
}