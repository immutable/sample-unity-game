using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using AudioSettings = HyperCasual.Core.AudioSettings;

namespace HyperCasual.Runner
{
    /// <summary>
    /// A simple class used to save a load values
    /// using PlayerPrefs.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        /// <summary>
        /// Returns the SaveManager.
        /// </summary>
        public static SaveManager Instance => s_Instance;
        static SaveManager s_Instance;

        const string k_LevelProgress = "LevelProgress";
        const string k_Food = "Food";
        const string k_Xp = "Xp";
        const string k_AudioSettings = "AudioSettings";
        const string k_QualityLevel = "QualityLevel";
        const string k_IsLoggedIn = "IsLoggedIn";
        const string k_UseNewSkin = "UseNewSkin";

        void Awake()
        {
            s_Instance = this;
        }

        /// <summary>
        /// Save and load level progress as an integer
        /// Level progress 2 means the player completed level 2, but has not completed level 3
        /// </summary>
        public int LevelProgress
        {
            get => PlayerPrefs.GetInt(k_LevelProgress);
            set => PlayerPrefs.SetInt(k_LevelProgress, value);
        }

        /// <summary>
        /// Save and load food as an integer
        /// </summary>
        public int Food
        {
            get => PlayerPrefs.GetInt(k_Food);
            set => PlayerPrefs.SetInt(k_Food, value);
        }

        public float XP
        {
            get => PlayerPrefs.GetFloat(k_Xp);
            set => PlayerPrefs.SetFloat(k_Xp, value);
        }

        public bool IsQualityLevelSaved => PlayerPrefs.HasKey(k_QualityLevel);

        public int QualityLevel
        {
            get => PlayerPrefs.GetInt(k_QualityLevel);
            set => PlayerPrefs.SetInt(k_QualityLevel, value);
        }

        public AudioSettings LoadAudioSettings()
        {
            return PlayerPrefsUtils.Read<AudioSettings>(k_AudioSettings);
        }

        public void SaveAudioSettings(AudioSettings audioSettings)
        {
            PlayerPrefsUtils.Write(k_AudioSettings, audioSettings);
        }

        public bool IsLoggedIn
        {
            get => PlayerPrefs.GetInt(k_IsLoggedIn) == 1;
            set => PlayerPrefs.SetInt(k_IsLoggedIn, value ? 1 : 0);
        }

        public bool UseNewSkin
        {
            get => PlayerPrefs.GetInt(k_UseNewSkin) == 1;
            set => PlayerPrefs.SetInt(k_UseNewSkin, value ? 1 : 0);
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}