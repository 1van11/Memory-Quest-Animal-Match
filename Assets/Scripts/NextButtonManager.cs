using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextButtonManager : MonoBehaviour
{
    [Tooltip("Scene name of the loading screen that leads to the next level")]
    public string loadingSceneName = "LoadingScreen";

    public void OnNextButtonPressed()
    {
        Debug.Log("▶️ Next button pressed.");
        UnlockNextLevel();
        StartCoroutine(LoadNextSceneWithDelay());
    }

    IEnumerator LoadNextSceneWithDelay()
    {
        // ✅ Wait a little to ensure PlayerPrefs.Save() completes
        yield return new WaitForSecondsRealtime(0.5f);

        Debug.Log("⏳ Loading next scene: " + loadingSceneName);
        SceneManager.LoadScene(loadingSceneName);
    }

    void UnlockNextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("📄 Current scene: " + currentScene);

        if (currentScene.StartsWith("LVL"))
        {
            string numberPart = currentScene.Replace("LVL", "").Trim();

            if (int.TryParse(numberPart, out int currentLevel))
            {
                int nextLevel = currentLevel + 1;
                PlayerPrefs.SetInt($"LevelUnlocked_{nextLevel}", 1);
                PlayerPrefs.Save(); // ✅ Force save immediately
                Debug.Log($"✅ LVL {nextLevel} unlocked and saved!");
            }
            else
            {
                Debug.LogWarning("⚠️ Could not parse level number from scene name.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Current scene name doesn't start with 'LVL'.");
        }
    }
}
