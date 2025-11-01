using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public Button[] levelButtons; // assign all level buttons in order (LVL 1, LVL 2, ...)

    void Start()
    {
        Debug.Log("🔄 LevelSelectManager started.");

        // Always unlock LVL 1
        if (PlayerPrefs.GetInt("LevelUnlocked_1", 0) == 0)
        {
            PlayerPrefs.SetInt("LevelUnlocked_1", 1);
            PlayerPrefs.Save();
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            bool unlocked = PlayerPrefs.GetInt($"LevelUnlocked_{levelIndex}", 0) == 1;
            levelButtons[i].interactable = unlocked;

            // Optional: grey out locked levels
            var txt = levelButtons[i].GetComponentInChildren<Text>();
            if (txt != null)
                txt.color = unlocked ? Color.white : Color.gray;

            Debug.Log($"🔍 LVL {levelIndex} unlocked? {unlocked}");
        }
    }

    public void LoadLevel(int levelNumber)
    {
        string sceneName = $"LVL {levelNumber}";
        Debug.Log("▶️ Loading " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    void Update()
{
    // Press R to reset progress while testing
    if (Input.GetKeyDown(KeyCode.R))
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("LevelUnlocked_1", 1);
        PlayerPrefs.Save();
        Debug.Log("🧹 Progress reset — Only Level 1 is unlocked!");
    }
}

}
