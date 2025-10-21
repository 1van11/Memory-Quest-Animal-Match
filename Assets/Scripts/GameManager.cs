using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI / Prefabs")]
    public Transform gridContainer;
    public GameObject cardPrefab;
    public Sprite backSprite;
    public GameObject summaryUI;

    [Header("Level 1 - Animal faces (2 unique)")]
    public Sprite[] animalSprites;

    [Header("UI References")]
    public TMP_Text scoreText;        // Gameplay score display
    public TMP_Text attemptText;      // Gameplay attempt display
    public TMP_Text timerText;        // Gameplay timer display

    [Header("Summary UI References")]
    public TMP_Text summaryScoreText;    // Summary: Final score
    public TMP_Text summaryAttemptText;  // Summary: Final attempts
    public TMP_Text summaryTimeText;     // Summary: Final time

    private int score = 0;
    private int attempts = 0;
    private float elapsedTime = 0f;
    private bool isPlaying = false;
    private bool isChecking = false;

    private List<CardController> revealedCards = new List<CardController>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {   
        Time.timeScale = 1;
        SetupLevel1();
        UpdateScoreText();
        UpdateAttemptText();
        UpdateTimerText();
        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timerText != null)
            timerText.text = formattedTime;
    }

    public void SetupLevel1()
    {
        if (animalSprites == null || animalSprites.Length < 2)
        {
            Debug.LogError("Assign 2 animal sprites for Level 1 in GameManager.");
            return;
        }

        int totalPairs = gridContainer.childCount / 2;
        List<Sprite> pool = new List<Sprite>();

        for (int i = 0; i < totalPairs; i++)
        {
        pool.Add(animalSprites[i % animalSprites.Length]);
        pool.Add(animalSprites[i % animalSprites.Length]);
        }

        Shuffle(pool);

        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        foreach (Sprite s in pool)
        {
            GameObject go = Instantiate(cardPrefab, gridContainer);
            CardController cc = go.GetComponent<CardController>();
            cc.frontSprite = s;
            cc.backSprite = backSprite;
            cc.ShowBackInstant();
        }

        if (summaryUI != null)
            summaryUI.SetActive(false);

        StartCoroutine(LevelIntroPeek());
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    IEnumerator LevelIntroPeek()
{
    yield return new WaitForSeconds(0.1f); // small delay so everything activates

    // Flip all cards to front
    foreach (Transform t in gridContainer)
    {
        CardController cc = t.GetComponent<CardController>();
        StartCoroutine(cc.FlipToFront());
    }

    // Wait while they’re showing
    yield return new WaitForSeconds(2f);

    // Flip them all back
    foreach (Transform t in gridContainer)
    {
        CardController cc = t.GetComponent<CardController>();
        StartCoroutine(cc.FlipToBack());
    }
}


    public void CardRevealed(CardController card)
    {
    // Don’t allow revealing more than 2 or during checking
    if (isChecking || revealedCards.Contains(card) || revealedCards.Count >= 2)
        return;

    revealedCards.Add(card);

    if (revealedCards.Count == 2)
        StartCoroutine(CheckMatch());
    }


    IEnumerator CheckMatch()
    {
    isChecking = true; // 🔒 lock new clicks
    yield return new WaitForSeconds(0.6f);

    attempts++;
    UpdateAttemptText();

    if (revealedCards[0].frontSprite == revealedCards[1].frontSprite)
    {
        revealedCards[0].SetMatched();
        revealedCards[1].SetMatched();
        AddScore(5);
    }
    else
    {
        StartCoroutine(revealedCards[0].FlipToBack());
        StartCoroutine(revealedCards[1].FlipToBack());
    }

    // small delay to avoid overlap during flipping
    yield return new WaitForSeconds(0.5f);

    revealedCards.Clear();
    isChecking = false; // 🔓 unlock input again
    CheckLevelComplete();
}

    void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    void UpdateAttemptText()
    {
        if (attemptText != null)
            attemptText.text = attempts.ToString();
    }

    void CheckLevelComplete()
    {
        bool allMatched = true;

        foreach (Transform t in gridContainer)
        {
            CardController cc = t.GetComponent<CardController>();
            if (!cc.IsMatched())
            {
                allMatched = false;
                break;
            }
        }

        if (allMatched)
        {
            isPlaying = false; // ✅ Stop timer
            Debug.Log("Level Complete!");

            if (summaryUI != null)
            {
                summaryUI.SetActive(true);
                ShowSummaryResults();
            }
        }
    }

    void ShowSummaryResults()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        string finalTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (summaryScoreText != null)
            summaryScoreText.text = score.ToString();

        if (summaryAttemptText != null)
            summaryAttemptText.text = attempts.ToString();

        if (summaryTimeText != null)
            summaryTimeText.text = finalTime;
    }

    public bool IsChecking()
    {
    return isChecking;
    }

}
