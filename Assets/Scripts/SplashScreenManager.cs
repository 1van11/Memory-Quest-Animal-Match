using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time in seconds to display splash screen")]
    public float splashDuration = 2f;
    
    [Tooltip("Name of the main menu scene")]
    public string mainMenuSceneName = "MainMenu";
    
    [Header("Optional Fade Effect")]
    public bool useFadeOut = true;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    void Start()
    {
        StartCoroutine(LoadMainMenuAfterDelay());
    }

    IEnumerator LoadMainMenuAfterDelay()
    {
        // Wait for splash duration
        yield return new WaitForSeconds(splashDuration);
        
        // Optional fade out effect
        if (useFadeOut && canvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }
        
        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}