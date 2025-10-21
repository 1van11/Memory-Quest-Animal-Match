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

    private List<CardController> revealedCards = new List<CardController>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
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

        List<Sprite> pool = new List<Sprite>
        {
            animalSprites[0], animalSprites[0],
            animalSprites[1], animalSprites[1]
        };

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
        yield return new WaitForEndOfFrame();

        foreach (Transform t in gridContainer)
        {
            CardController cc = t.GetComponent<CardController>();
            StartCoroutine(cc.FlipToFront());
        }

        yield return new WaitForSeconds(2f);

        foreach (Transform t in gridContainer)
        {
            CardController cc = t.GetComponent<CardController>();
            StartCoroutine(cc.FlipToBack());
        }
    }

    public void CardRevealed(CardController card)
    {
        revealedCards.Add(card);

        if (revealedCards.Count == 2)
            StartCoroutine(CheckMatch());
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.6f);

        // ✅ Every two revealed cards = 1 attempt
        attempts++;
        UpdateAttemptText();

        if (revealedCards[0].frontSprite == revealedCards[1].frontSprite)
        {
            revealedCards[0].SetMatched();
            revealedCards[1].SetMatched();
            AddScore(5); // ✅ +5 points for correct match
        }
        else
        {
            StartCoroutine(revealedCards[0].FlipToBack());
            StartCoroutine(revealedCards[1].FlipToBack());
        }

        revealedCards.Clear();
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
}
