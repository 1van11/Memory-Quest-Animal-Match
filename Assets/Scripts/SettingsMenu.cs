using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio Mixer Reference")]
    [SerializeField] private AudioMixer audioMixer;

    private const string MusicKey = "music";
    private const string SFXKey = "sfx";

    private bool isPaused = false;

    private void Start()
    {
        // Hide settings panel by default
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Button listeners
        settingsButton?.onClick.AddListener(OpenSettings);
        closeButton?.onClick.AddListener(CloseSettings);

        // Slider listeners
        musicSlider?.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider?.onValueChanged.AddListener(SetSFXVolume);

        // Load saved values
        float musicVol = PlayerPrefs.GetFloat(MusicKey, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFXKey, 1f);

        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }

    private void OpenSettings()
    {
        settingsPanel?.SetActive(true);
        Time.timeScale = 0f; // Pause game
        isPaused = true;
    }

    private void CloseSettings()
    {
        settingsPanel?.SetActive(false);
        Time.timeScale = 1f; // Resume game
        isPaused = false;
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat(MusicKey, dB);
        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat(SFXKey, dB);
        PlayerPrefs.SetFloat(SFXKey, value);
    }

    private void OnEnable()
    {
        if (audioMixer.GetFloat(MusicKey, out float currentMusicDb))
            musicSlider.value = Mathf.Pow(10f, currentMusicDb / 20f);

        if (audioMixer.GetFloat(SFXKey, out float currentSfxDb))
            sfxSlider.value = Mathf.Pow(10f, currentSfxDb / 20f);
    }
}
