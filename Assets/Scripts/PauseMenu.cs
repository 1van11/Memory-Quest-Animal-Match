using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public Button pauseButton;   
    public Button resumeButton;
    public Button homeButton;

    private bool isPaused = false;

    void Start()
    {
        // Hide pause panel on start
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Button listeners
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (homeButton != null)
            homeButton.onClick.AddListener(ReturnToHome);
    }

    void Update()
    {
        // Optional: allow keyboard pause too
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        // ✅ Wait one frame before freezing time to let the click finish properly
        StartCoroutine(DoPause());
    }

    private System.Collections.IEnumerator DoPause()
    {
        yield return null; // wait 1 frame so button click fully registers

        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Optional: disable pause button only AFTER showing panel
        if (pauseButton != null)
            pauseButton.interactable = false;

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.interactable = true;

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ReturnToHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Mainmenu");
    }
}
