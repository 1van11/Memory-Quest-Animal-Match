using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

    private const string MusicVolumeKey = "musicVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicSlider.value = savedVolume;
        SetMusicVolume(savedVolume);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        myMixer.SetFloat("music", dB);


        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    private void OnEnable()
    {
        if (myMixer.GetFloat("music", out float currentDB))
        {
            float linear = Mathf.Pow(10f, currentDB / 20f);
            musicSlider.value = linear;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
