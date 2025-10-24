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

    [Header("Animal Sounds")]
    public AudioSource audioSource;
    public AudioClip catSound;
    public AudioClip dogSound;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text attemptText;
    public TMP_Text timerText;

    [Header("Summary UI References")]
    public TMP_Text summaryScoreText;
    public TMP_Text summaryAttemptText;
    public TMP_Text summaryTimeText;

    [Header("Fun Fact UI")]
    public GameObject funFactPanel;
    public TMP_Text funFactText;

    private int score = 0;
    private int attempts = 0;
    private float elapsedTime = 0f;
    private bool isPlaying = false;
    private bool isChecking = false;
    private bool isShowingFunFact = false;
    private bool canInteract = true;

    private List<CardController> revealedCards = new List<CardController>();
    private Dictionary<string, string> funFacts = new Dictionary<string, string>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1;
        SetupFunFacts();
        SetupLevel1();
        UpdateScoreText();
        UpdateAttemptText();
        UpdateTimerText();
    }

    void SetupFunFacts()
    {
        funFacts.Add("Cat", "Cats sleep for about 70% of their lives. They can also make over 100 different sounds.");
        funFacts.Add("Dog", "A dog's sense of smell is up to 100,000 times stronger than a human’s. They can also learn more than 1,000 words.");
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

    public void OnClick()
{
    // Stop clicking while fun fact or checking pairs
    if (!GameManager.instance.CanInteract() || GameManager.instance.IsChecking())
        return;

    // Your existing flip logic here
}

    public void SetupLevel1()
    {
        if (animalSprites == null || animalSprites.Length < 2)
        {
            Debug.LogError("Assign at least 2 animal sprites for Level 1 in GameManager.");
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

        if (funFactPanel != null)
            funFactPanel.SetActive(false);

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
        yield return new WaitForSeconds(0.1f);

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

        yield return new WaitForSeconds(0.5f);
        isPlaying = true;
    }

    public void CardRevealed(CardController card)
    {
        if (isChecking || revealedCards.Contains(card) || revealedCards.Count >= 2)
            return;

        revealedCards.Add(card);

        if (revealedCards.Count == 2)
            StartCoroutine(CheckMatch());
    }

    IEnumerator CheckMatch()
    {
        isChecking = true;
        yield return new WaitForSeconds(0.6f);

        attempts++;
        UpdateAttemptText();

        if (revealedCards[0].frontSprite == revealedCards[1].frontSprite)
        {
            revealedCards[0].SetMatched();
            revealedCards[1].SetMatched();
            AddScore(5);

            string animalName = revealedCards[0].frontSprite.name;

            PlayAnimalSound(animalName);

            if (funFacts.ContainsKey(animalName))
            {
                ShowFunFact(funFacts[animalName]);
            }
        }
        else
        {
            StartCoroutine(revealedCards[0].FlipToBack());
            StartCoroutine(revealedCards[1].FlipToBack());
        }

        yield return new WaitForSeconds(0.5f);

        revealedCards.Clear();
        isChecking = false;

        StartCoroutine(WaitForFunFactThenCheckComplete());
    }

    IEnumerator WaitForFunFactThenCheckComplete()
    {
        while (isShowingFunFact)
        {
            yield return null;
        }
        CheckLevelComplete();
    }

    void PlayAnimalSound(string animalName)
    {
        if (audioSource == null) return;

        if (animalName == "Cat" && catSound != null)
            audioSource.PlayOneShot(catSound);
        else if (animalName == "Dog" && dogSound != null)
            audioSource.PlayOneShot(dogSound);
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
            isPlaying = false;
            Debug.Log("Level Complete!");
            StartCoroutine(ShowSummaryAfterDelay());
        }
    }

    IEnumerator ShowSummaryAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        isPlaying = false;

        if (summaryUI != null)
        {
            summaryUI.SetActive(true);
            ShowSummaryResults();
        }
    }

    void ShowSummaryResults()
{
    // Start the typing animation for score, attempts, and time
    StartCoroutine(TypeSummaryResults());
}


    Coroutine typingCoroutine;

void ShowFunFact(string fact)
{
    if (funFactPanel != null && funFactText != null)
    {
        isPlaying = false;
        isShowingFunFact = true;

        // 🚫 Disable card clicks
        SetCardInteractivity(false);

        funFactPanel.SetActive(true);

        // Stop only the typing coroutine (not everything!)
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Start typing animation
        typingCoroutine = StartCoroutine(TypeFunFact(fact));
    }
}

IEnumerator TypeFunFact(string fact)
{
    funFactText.text = "";
    float typingSpeed = 0.03f; // ⏱ speed between each character

    foreach (char letter in fact)
    {
        funFactText.text += letter;
        yield return new WaitForSeconds(typingSpeed);
    }

    // Wait a bit after typing finishes
    yield return new WaitForSeconds(2.5f);

    StartCoroutine(HideFunFactAfterDelay());
}

IEnumerator HideFunFactAfterDelay()
{
    yield return new WaitForSeconds(0.3f);

    if (funFactPanel != null)
        funFactPanel.SetActive(false);

    // ▶️ Resume timer and enable card clicks again
    isPlaying = true;
    isShowingFunFact = false;
    SetCardInteractivity(true);
}


    public bool IsChecking()
    {
        return isChecking;
    }

    public void SetCardInteractivity(bool state)
{
    canInteract = state;
}

public bool CanInteract()
{
    return canInteract;
}

IEnumerator TypeSummaryResults()
{
    if (summaryScoreText == null || summaryAttemptText == null || summaryTimeText == null)
        yield break;

    summaryScoreText.text = "";
    summaryAttemptText.text = "";
    summaryTimeText.text = "";

    float typingSpeed = 0.03f;

    string scoreString = score.ToString();
    int minutes = Mathf.FloorToInt(elapsedTime / 60);
    int seconds = Mathf.FloorToInt(elapsedTime % 60);
    string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
    string attemptString = attempts.ToString();

    // 🧮 Type the score first
    foreach (char c in scoreString)
    {
        summaryScoreText.text += c;
        yield return new WaitForSeconds(typingSpeed);
    }

    yield return new WaitForSeconds(0.3f);

    // ⏱ Then type the time
    foreach (char c in timeString)
    {
        summaryTimeText.text += c;
        yield return new WaitForSeconds(typingSpeed);
    }

    yield return new WaitForSeconds(0.3f);

    // 🧠 Finally type the attempts
    foreach (char c in attemptString)
    {
        summaryAttemptText.text += c;
        yield return new WaitForSeconds(typingSpeed);
    }
}

}
