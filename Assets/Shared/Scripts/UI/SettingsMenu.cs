using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     This View contains settings menu functionalities
    /// </summary>
    public class SettingsMenu : View
    {
        [SerializeField] private HyperCasualButton m_Button;

        [SerializeField] private Toggle m_EnableMusicToggle;

        [SerializeField] private Toggle m_EnableSfxToggle;

        [SerializeField] private Slider m_AudioVolumeSlider;

        [SerializeField] private Slider m_QualitySlider;

        private void OnEnable()
        {
            m_EnableMusicToggle.isOn = AudioManager.Instance.EnableMusic;
            m_EnableSfxToggle.isOn = AudioManager.Instance.EnableSfx;
            m_AudioVolumeSlider.value = AudioManager.Instance.MasterVolume;
            m_QualitySlider.value = QualityManager.Instance.QualityLevel;

            m_Button.AddListener(OnBackButtonClick);
            m_EnableMusicToggle.onValueChanged.AddListener(MusicToggleChanged);
            m_EnableSfxToggle.onValueChanged.AddListener(SfxToggleChanged);
            m_AudioVolumeSlider.onValueChanged.AddListener(VolumeSliderChanged);
            m_QualitySlider.onValueChanged.AddListener(QualitySliderChanged);
        }

        private void OnDisable()
        {
            m_Button.RemoveListener(OnBackButtonClick);
            m_EnableMusicToggle.onValueChanged.RemoveListener(MusicToggleChanged);
            m_EnableSfxToggle.onValueChanged.RemoveListener(SfxToggleChanged);
            m_AudioVolumeSlider.onValueChanged.RemoveListener(VolumeSliderChanged);
            m_QualitySlider.onValueChanged.RemoveListener(QualitySliderChanged);
        }

        private void MusicToggleChanged(bool value)
        {
            AudioManager.Instance.EnableMusic = value;
        }

        private void SfxToggleChanged(bool value)
        {
            AudioManager.Instance.EnableSfx = value;
        }

        private void VolumeSliderChanged(float value)
        {
            AudioManager.Instance.MasterVolume = value;
        }

        private void QualitySliderChanged(float value)
        {
            QualityManager.Instance.QualityLevel = (int)value;
        }

        private void OnBackButtonClick()
        {
            UIManager.Instance.GoBack();
        }
    }
}