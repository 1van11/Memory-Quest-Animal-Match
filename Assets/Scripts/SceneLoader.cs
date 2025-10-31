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

    [Tooltip("Assign your Quit Button here.")]
    public Button quitButton;

    private void Start()
    {
        // Setup play button
        if (playButton != null)
            playButton.onClick.AddListener(LoadScene);
        else
            Debug.LogWarning("⚠️ Play button not assigned in SceneLoader!");

        // Setup quit button
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        else
            Debug.LogWarning("⚠️ Quit button not assigned in SceneLoader!");
    }

    // === PLAY BUTTON ===
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

    // === QUIT BUTTON (Instant) ===
    public void QuitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

#if UNITY_EDITOR
        // Stop Play Mode when testing in Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
