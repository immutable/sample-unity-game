using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     A level editor window that allows the user to
    ///     load levels in the level editor scene, modify level
    ///     parameters, and save that level to be loaded in
    ///     a Runner game.
    /// </summary>
    public class RunnerLevelEditorWindow : EditorWindow
    {
        private const string k_EditorPrefsPreviouslyLoadedLevelPath = "PreviouslyLoadedLevelPath";

        private const string k_AutoSaveSettingsInitializedKey = "AutoSaveInitialized";
        private const string k_AutoSaveLevelKey = "AutoSaveLevel";
        private const string k_AutoSavePlayerKey = "AutoSavePlayer";
        private const string k_AutoSaveCameraKey = "AutoSaveCamera";
        private const string k_AutoSaveShowSettingsKey = "AutoSaveShowSettings";

        private const string k_LevelParentGameObjectName = "LevelParent";
        private const string k_LevelEditorSceneName = "RunnerLevelEditor";
        private const string k_LevelEditorScenePath = "Assets/Runner/Scenes/RunnerLevelEditor.unity";

        private static readonly Color s_Blue = new(0.0f, 0.0f, 1.0f, 1.0f);
        private static readonly string s_LevelParentTag = "LevelParent";

        private readonly List<Spawnable> m_SelectedSpawnables = new();
        private Color m_ActiveColor;
        private bool m_AttemptedToLoadPreviousLevel;
        private bool m_AutoSaveCamera;
        private bool m_AutoSaveLevel;
        private bool m_AutoSavePlayer;

        private bool m_AutoSaveSettingsLoaded;
        private bool m_AutoSaveShowSettings;
        private bool m_CurrentLevelNotLoaded;
        private GameObject m_LevelMarkersGO;

        private GameObject m_LevelParentGO;
        private GameObject m_LoadedLevelGO;
        private GameObject m_TerrainGO;
        internal bool HasLoadedLevel { get; private set; }
        internal LevelDefinition SourceLevelDefinition { get; private set; }

        /// <summary>
        ///     Returns the loaded LevelDefinition.
        /// </summary>
        public LevelDefinition LoadedLevelDefinition { get; private set; }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
        }

        private void OnGUI()
        {
            if (!m_AutoSaveSettingsLoaded)
                // Load auto-save settings
                LoadAutoSaveSettings();

            if (Application.isPlaying)
            {
                GUILayout.Label("Exit play mode to continue editing level.");
                return;
            }

            var scene = SceneManager.GetActiveScene();
            if (!scene.name.Equals(k_LevelEditorSceneName))
            {
                if (GUILayout.Button("Open Level Editor Scene"))
                {
                    EditorSceneManager.OpenScene(k_LevelEditorScenePath);
                    if (SourceLevelDefinition != null) LoadLevel(SourceLevelDefinition);
                }

                return;
            }

            SourceLevelDefinition = (LevelDefinition)EditorGUILayout.ObjectField("Level Definition",
                SourceLevelDefinition, typeof(LevelDefinition), false, null);

            if (SourceLevelDefinition == null)
            {
                GUILayout.Label("Select a LevelDefinition ScriptableObject to begin.");
                HasLoadedLevel = false;
                return;
            }

            if (LoadedLevelDefinition != null && !SourceLevelDefinition.name.Equals(LoadedLevelDefinition.name))
            {
                // Automatically load the new source level if it has changed.
                LoadLevel(SourceLevelDefinition);
                return;
            }

            if (Event.current.type == EventType.Layout) m_CurrentLevelNotLoaded = LevelNotLoaded();

            if (m_LoadedLevelGO != null && !m_CurrentLevelNotLoaded)
            {
                if (GUILayout.Button("Reload Level")) LoadLevel(SourceLevelDefinition);
            }
            else
            {
                LoadLevel(SourceLevelDefinition);
            }

            if (LoadedLevelDefinition == null || m_CurrentLevelNotLoaded)
            {
                GUILayout.Label("No level loaded.");
                return;
            }

            if (GUILayout.Button("Save Level")) SaveLevel(LoadedLevelDefinition);

            // Auto-save

            m_AutoSaveShowSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(m_AutoSaveShowSettings, "Auto-Save Settings");

            if (m_AutoSaveShowSettings)
            {
                EditorGUI.BeginChangeCheck();
                m_AutoSaveLevel =
                    EditorGUILayout.Toggle(
                        new GUIContent("Save Level on Play",
                            "Any changes made to the level being edited will be automatically saved when entering play mode."),
                        m_AutoSaveLevel);
                m_AutoSavePlayer = EditorGUILayout.Toggle(
                    new GUIContent("Save Player on Play",
                        "Any changes made to the Player prefab will be automatically saved when entering play mode and reflected when playing the game via the Boot scene."),
                    m_AutoSavePlayer);
                m_AutoSaveCamera = EditorGUILayout.Toggle(
                    new GUIContent("Save Camera on Play",
                        "Any changes made to the GameplayCamera prefab will be automatically saved when entering play mode and reflected when playing the game via the Boot scene."),
                    m_AutoSaveCamera);
                if (EditorGUI.EndChangeCheck()) SaveAutoSaveSettings();
            }

            EditorGUILayout.Space();

            // Level Size Parameters

            GUILayout.Label("Terrain", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            LoadedLevelDefinition.LevelLength = Mathf.Max(0.0f,
                EditorGUILayout.FloatField("Length", LoadedLevelDefinition.LevelLength));
            LoadedLevelDefinition.LevelWidth =
                Mathf.Max(0.0f, EditorGUILayout.FloatField("Width", LoadedLevelDefinition.LevelWidth));
            LoadedLevelDefinition.LevelLengthBufferStart = Mathf.Max(0.0f,
                EditorGUILayout.FloatField("Start Buffer", LoadedLevelDefinition.LevelLengthBufferStart));
            LoadedLevelDefinition.LevelLengthBufferEnd = Mathf.Max(0.0f,
                EditorGUILayout.FloatField("End Buffer", LoadedLevelDefinition.LevelLengthBufferEnd));
            LoadedLevelDefinition.LevelThickness =
                Mathf.Max(EditorGUILayout.FloatField("Level Thickness", LoadedLevelDefinition.LevelThickness));
            LoadedLevelDefinition.TerrainMaterial = (Material)EditorGUILayout.ObjectField("Terrain Material",
                LoadedLevelDefinition.TerrainMaterial, typeof(Material), false, null);
            if (EditorGUI.EndChangeCheck() && m_TerrainGO != null && m_LevelParentGO != null)
            {
                GameManager.CreateTerrain(LoadedLevelDefinition, ref m_TerrainGO);
                m_TerrainGO.transform.SetParent(m_LevelParentGO.transform);
            }

            EditorGUILayout.Space();

            // Spawnable Snapping

            GUILayout.Label("Snapping Options", EditorStyles.boldLabel);
            LoadedLevelDefinition.SnapToGrid = EditorGUILayout.Toggle("Snap to Grid", LoadedLevelDefinition.SnapToGrid);
            if (LoadedLevelDefinition.SnapToGrid)
                // Ensure the grid size is never too small, zero, or negative
                LoadedLevelDefinition.GridSize = Mathf.Max(0.1f,
                    EditorGUILayout.FloatField("Grid Size", LoadedLevelDefinition.GridSize));
            EditorGUILayout.Space();

            // Necessary Prefabs

            GUILayout.Label("Prefabs", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            LoadedLevelDefinition.StartPrefab = (GameObject)EditorGUILayout.ObjectField("Start Prefab",
                LoadedLevelDefinition.StartPrefab, typeof(GameObject), false, null);
            LoadedLevelDefinition.EndPrefab = (GameObject)EditorGUILayout.ObjectField("End Prefab",
                LoadedLevelDefinition.EndPrefab, typeof(GameObject), false, null);
            if (EditorGUI.EndChangeCheck())
            {
                GameManager.PlaceLevelMarkers(LoadedLevelDefinition, ref m_LevelMarkersGO);
                m_LevelMarkersGO.transform.SetParent(m_LevelParentGO.transform);
            }

            EditorGUILayout.Space();

            // Spawnable Coloring
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                m_SelectedSpawnables.Clear();
                for (var i = 0; i < Selection.gameObjects.Length; i++)
                {
                    var spawnable = Selection.gameObjects[i].GetComponent<Spawnable>();
                    if (spawnable != null && PrefabUtility.IsPartOfNonAssetPrefabInstance(Selection.gameObjects[i]))
                        m_SelectedSpawnables.Add(spawnable);
                }

                if (m_SelectedSpawnables.Count > 0)
                {
                    GUILayout.Label("Selected Spawnable Options", EditorStyles.boldLabel);
                    m_ActiveColor = EditorGUILayout.ColorField("Base Color", m_ActiveColor);
                    if (GUILayout.Button("Apply Base Color to Selected Spawnables"))
                        for (var i = 0; i < m_SelectedSpawnables.Count; i++)
                            m_SelectedSpawnables[i].SetBaseColor(m_ActiveColor);
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.HelpBox(
                $"New objects added to the level require a {nameof(Spawnable)} type component added to the GameObject",
                MessageType.Info);
        }

        private void OnFocus()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            EditorSceneManager.sceneSaved -= OnSceneSaved;
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        private void OnSelectionChange()
        {
            // Needed to update color options when a Spawnable is selected
            Repaint();
        }

        [MenuItem("Window/Runner Level Editor")]
        private static void Init()
        {
            var window = (RunnerLevelEditorWindow)GetWindow(typeof(RunnerLevelEditorWindow), false, "Level Editor");
            window.Show();

            // Load auto-save settings
            window.LoadAutoSaveSettings();
        }

        /// <summary>
        ///     Load the auto-save settings from EditorPrefs.
        /// </summary>
        public void LoadAutoSaveSettings()
        {
            var autoSaveSettingsInitialized = EditorPrefs.GetBool(k_AutoSaveSettingsInitializedKey);

            if (!autoSaveSettingsInitialized)
            {
                // Default all auto-save values to true and save them to Editor Prefs
                // the first time the user opens the window

                m_AutoSaveLevel = true;
                m_AutoSavePlayer = true;
                m_AutoSaveCamera = true;

                EditorPrefs.SetBool(k_AutoSaveLevelKey, m_AutoSaveLevel);
                EditorPrefs.SetBool(k_AutoSavePlayerKey, m_AutoSavePlayer);
                EditorPrefs.SetBool(k_AutoSaveCameraKey, m_AutoSaveCamera);

                EditorPrefs.SetBool(k_AutoSaveSettingsInitializedKey, true);
                return;
            }

            m_AutoSaveShowSettings = EditorPrefs.GetBool(k_AutoSaveShowSettingsKey);
            m_AutoSaveLevel = EditorPrefs.GetBool(k_AutoSaveLevelKey);
            m_AutoSavePlayer = EditorPrefs.GetBool(k_AutoSavePlayerKey);
            m_AutoSaveCamera = EditorPrefs.GetBool(k_AutoSaveCameraKey);

            m_AutoSaveSettingsLoaded = true;
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if ((state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode) &&
                SourceLevelDefinition != null)
            {
                var scene = SceneManager.GetActiveScene();
                if (scene.name.Equals(k_LevelEditorSceneName))
                    // Reload the scene automatically
                    LoadLevel(SourceLevelDefinition);
            }
            else if (state == PlayModeStateChange.ExitingEditMode && SourceLevelDefinition != null && !LevelNotLoaded())
            {
                var scene = SceneManager.GetActiveScene();
                if (scene.name.Equals(k_LevelEditorSceneName))
                    // Save the scene automatically before testing
                    SaveLevel(LoadedLevelDefinition);
            }
        }

        private void OnSceneSaved(Scene scene)
        {
            if (SourceLevelDefinition != null && !LevelNotLoaded())
                if (scene.name.Equals(k_LevelEditorSceneName))
                    SaveLevel(LoadedLevelDefinition);
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (LoadedLevelDefinition == null)
            {
                var levelPath = EditorPrefs.GetString(k_EditorPrefsPreviouslyLoadedLevelPath);
                var levelPathExists = !string.IsNullOrEmpty(levelPath);

                // Attempt to load previously loaded level
                if (!m_AttemptedToLoadPreviousLevel && levelPathExists)
                {
                    SourceLevelDefinition = AssetDatabase.LoadAssetAtPath<LevelDefinition>(levelPath);

                    if (SourceLevelDefinition != null)
                    {
                        LoadLevel(SourceLevelDefinition);
                    }
                    else
                    {
                        Debug.LogError(
                            $"Could not load level with path {levelPath}. Specify a valid level to continue.");
                        m_AttemptedToLoadPreviousLevel = true;
                    }
                }
                else if (levelPathExists)
                {
                    Debug.LogError($"Could not load level with path {levelPath}. Specify a valid level to continue.");
                    m_AttemptedToLoadPreviousLevel = true;
                }

                return;
            }

            if (LoadedLevelDefinition.SnapToGrid)
            {
                var nearestGridPositionToLevelWidth = LoadedLevelDefinition.LevelWidth +
                                                      LoadedLevelDefinition.LevelWidth % LoadedLevelDefinition.GridSize;
                var nearestGridPositionToLevelLength = LoadedLevelDefinition.LevelLength +
                                                       LoadedLevelDefinition.LevelLength %
                                                       LoadedLevelDefinition.GridSize;

                var numberOfGridLinesWide =
                    (int)Mathf.Ceil(nearestGridPositionToLevelWidth / LoadedLevelDefinition.GridSize);
                var numberOfGridLinesLong =
                    (int)Mathf.Ceil(nearestGridPositionToLevelLength / LoadedLevelDefinition.GridSize);

                Handles.BeginGUI();
                Handles.color = s_Blue;

                // Empty label is needed to draw lines below
                Handles.Label(Vector3.zero, "");

                var gridWidth = numberOfGridLinesWide * LoadedLevelDefinition.GridSize;
                var gridLength = numberOfGridLinesLong * LoadedLevelDefinition.GridSize;

                // Draw horizontal grid lines (parallel to X axis) from the start 
                // of the level to the end of the level
                for (var z = 0; z <= numberOfGridLinesLong; z++)
                {
                    var zPosition = z * LoadedLevelDefinition.GridSize;
                    Handles.DrawLine(new Vector3(-gridWidth, 0.0f, zPosition), new Vector3(gridWidth, 0.0f, zPosition));
                }

                // Draw vertical grid lines (parallel to Z axis) from the center out
                for (var x = 0; x <= numberOfGridLinesWide; x++)
                {
                    var xPosition = x * LoadedLevelDefinition.GridSize;
                    Handles.DrawLine(new Vector3(-xPosition, 0.0f, 0.0f), new Vector3(-xPosition, 0.0f, gridLength));

                    // Only draw one grid line at the center of the level
                    if (x > 0)
                        Handles.DrawLine(new Vector3(xPosition, 0.0f, 0.0f), new Vector3(xPosition, 0.0f, gridLength));
                }

                Handles.EndGUI();
            }
        }

        private bool LevelNotLoaded()
        {
            return LoadedLevelDefinition == null || m_LevelParentGO == null || m_LoadedLevelGO == null ||
                   m_TerrainGO == null || m_LevelMarkersGO == null;
        }

        private void LoadLevel(LevelDefinition levelDefinition)
        {
            UnloadOpenLevels();

            if (!SceneManager.GetActiveScene().path.Equals(k_LevelEditorScenePath))
                return;

            LoadedLevelDefinition = Instantiate(levelDefinition);
            LoadedLevelDefinition.name = levelDefinition.name;

            m_LevelParentGO = new GameObject(k_LevelParentGameObjectName);
            m_LevelParentGO.tag = s_LevelParentTag;

            GameManager.LoadLevel(LoadedLevelDefinition, ref m_LoadedLevelGO);
            GameManager.CreateTerrain(LoadedLevelDefinition, ref m_TerrainGO);
            GameManager.PlaceLevelMarkers(LoadedLevelDefinition, ref m_LevelMarkersGO);

            m_LoadedLevelGO.transform.SetParent(m_LevelParentGO.transform);
            m_TerrainGO.transform.SetParent(m_LevelParentGO.transform);
            m_LevelMarkersGO.transform.SetParent(m_LevelParentGO.transform);
            HasLoadedLevel = true;

            var levelPath = AssetDatabase.GetAssetPath(levelDefinition);
            EditorPrefs.SetString(k_EditorPrefsPreviouslyLoadedLevelPath, levelPath);

            m_AttemptedToLoadPreviousLevel = false;

            Repaint();
        }

        private void UnloadOpenLevels()
        {
            var levelParents = GameObject.FindGameObjectsWithTag(s_LevelParentTag);
            for (var i = 0; i < levelParents.Length; i++) DestroyImmediate(levelParents[i]);

            m_LevelParentGO = null;
        }

        private void SaveLevel(LevelDefinition levelDefinition)
        {
            if (m_AutoSaveLevel)
            {
                // Update array of spawnables based on what is currently in the scene
                var spawnables = (Spawnable[])FindObjectsOfType(typeof(Spawnable));
                levelDefinition.Spawnables = new LevelDefinition.SpawnableObject[spawnables.Length];
                for (var i = 0; i < spawnables.Length; i++)
                    try
                    {
                        levelDefinition.Spawnables[i] = new LevelDefinition.SpawnableObject
                        {
                            SpawnablePrefab =
                                PrefabUtility.GetCorrespondingObjectFromOriginalSource(spawnables[i].gameObject),
                            Position = spawnables[i].SavedPosition,
                            EulerAngles = spawnables[i].transform.eulerAngles,
                            Scale = spawnables[i].transform.lossyScale,
                            BaseColor = spawnables[i].BaseColor
                        };
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }

                // Overwrite source level definition with current version
                SourceLevelDefinition.SaveValues(levelDefinition);
            }

            if (m_AutoSavePlayer)
            {
                var players = (PlayerController[])FindObjectsOfType(typeof(PlayerController));
                if (players.Length == 1)
                {
                    var playerPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(players[0].gameObject);
                    if (playerPrefab != null)
                        PrefabUtility.ApplyPrefabInstance(players[0].gameObject, InteractionMode.UserAction);
                    else
                        Debug.LogError(
                            "PlayerController could not be found on a prefab instance. Changes could not be saved.");
                }
                else
                {
                    if (players.Length == 0)
                        Debug.LogWarning("No instance of PlayerController found in the scene. No changes saved!");
                    else
                        Debug.LogWarning(
                            "More than two instances of PlayerController found in the scene. No changes saved!");
                }
            }

            if (m_AutoSaveCamera)
            {
                var cameraManagers = (CameraManager[])FindObjectsOfType(typeof(CameraManager));
                if (cameraManagers.Length == 1)
                {
                    var cameraManagerPrefab =
                        PrefabUtility.GetCorrespondingObjectFromOriginalSource(cameraManagers[0].gameObject);
                    if (cameraManagerPrefab != null)
                        PrefabUtility.ApplyPrefabInstance(cameraManagers[0].gameObject, InteractionMode.UserAction);
                    else
                        Debug.LogError(
                            "CameraManager could not be found on a prefab instance. Changes could not be saved.");
                }
                else
                {
                    if (cameraManagers.Length == 0)
                        Debug.LogWarning("No instance of CameraManager found in the scene. No changes saved!");
                    else
                        Debug.LogWarning(
                            "More than two instances of CameraManager found in the scene. No changes saved!");
                }
            }

            // Set level definition dirty so the changes will be written to disk
            EditorUtility.SetDirty(SourceLevelDefinition);

            // Write changes to disk
            AssetDatabase.SaveAssets();
        }

        private void SaveAutoSaveSettings()
        {
            // Write auto-save settings to EditorPrefs
            EditorPrefs.SetBool(k_AutoSaveLevelKey, m_AutoSaveLevel);
            EditorPrefs.SetBool(k_AutoSavePlayerKey, m_AutoSavePlayer);
            EditorPrefs.SetBool(k_AutoSaveCameraKey, m_AutoSaveCamera);
            EditorPrefs.SetBool(k_AutoSaveShowSettingsKey, m_AutoSaveShowSettings);
        }
    }
}