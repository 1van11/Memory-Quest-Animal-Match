using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Enter the name of the scene to load when Play is clicked.")]
    public string sceneToLoad;

    [Header("UI References")]
    [Tooltip("Assign your Play Button here.")]
    public Button playButton;

    void Start()
    {
        if (playButton != null)
        {
            // Attach the LoadScene function to the button’s onClick event
            playButton.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogWarning("⚠️ Play button not assigned in SceneLoader!");
        }
    }

    public void LoadScene()
    {   
        Time.timeScale = 1;
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("❌ Scene name is empty! Please assign a scene name in the Inspector.");
        }
    }
}
