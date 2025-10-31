using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

    private const string MusicVolumeKey = "musicVolume";

    private void Awake()
    {
        // Load saved volume before Start runs (useful when changing scenes)
        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
            SetMusicVolume(savedVolume);
        }
    }

    private void Start()
    {
        // Load slider position
        float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicSlider.value = savedVolume;
        SetMusicVolume(savedVolume);

        // Add listener for real-time updates
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMusicVolume(float value)
    {
        // Prevent log10(0) issues
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        myMixer.SetFloat("music", dB);

        // Save volume
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save(); // Save immediately for persistence
    }

    private void OnEnable()
    {
        // Sync slider when re-enabling the settings panel
        if (myMixer.GetFloat("music", out float currentDB))
        {
            float linear = Mathf.Pow(10f, currentDB / 20f);
            musicSlider.value = linear;
        }
    }
}
