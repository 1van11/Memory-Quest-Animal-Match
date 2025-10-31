using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenLoader : MonoBehaviour
{
    [Header("UI References")]
    public Image loadingBarFill;      
    public TMP_Text loadingText;      

    [Header("Settings")]
    public string sceneToLoad = "LVL";
    public float totalDuration = 3f;  

    void Start()
    {
        StartCoroutine(LoadSceneSmooth());
    }

    IEnumerator LoadSceneSmooth()
    {
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / totalDuration);

            // Move bar to the right over time
            loadingBarFill.fillAmount = progress;

            // Update text
            loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
