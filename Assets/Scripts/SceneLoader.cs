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

    [Tooltip("Assign your Quit Confirmation Panel here.")]
    public GameObject quitConfirmPanel;

    private void Start()
    {
        // Setup play button
        if (playButton != null)
            playButton.onClick.AddListener(LoadScene);
        else
            Debug.LogWarning("⚠️ Play button not assigned in SceneLoader!");

        // Setup quit button
        if (quitButton != null)
            quitButton.onClick.AddListener(ShowQuitConfirmation);
        else
            Debug.LogWarning("⚠️ Quit button not assigned in SceneLoader!");

        // Hide quit confirmation panel on start
        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);
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

    // === QUIT BUTTON ===
    public void ShowQuitConfirmation()
    {
        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(true);
    }

    public void ConfirmQuit()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

        // Just for testing in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CancelQuit()
    {
        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);
    }
}
