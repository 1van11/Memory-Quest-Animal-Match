using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


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
    public AudioClip pigSound;
    public AudioClip snakeSound;
    public AudioClip bearSound;
    public AudioClip chickenSound;
    public AudioClip cowSound;
    public AudioClip duckSound;
    public AudioClip elephantSound;
    public AudioClip foxSound;
    public AudioClip frogSound;
    public AudioClip HedgehogSound;
    public AudioClip horseSound;
    public AudioClip koalaSound;
    public AudioClip lionSound;
    public AudioClip mouseSound;
    public AudioClip owlSound;
    public AudioClip pandaSound;

    [Header("Timer Sound")]
    public AudioSource timerAudioSource;
    public AudioClip timerSoundLoop;

    [Header("Background Music")]
    public AudioSource bgMusicSource;

    [Header("You Win Sound")]
    public AudioClip youWinSound;
    public AudioSource sfxAudioSource;

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

    [Header("SFX")]
    public AudioClip flipCardSound;     // assign your flip sound clip here
    public AudioClip wrongPairSound;

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
        funFacts.Add("Pig", "A pig can play video games better than some humans. They also love belly rubs and can recognize their own names when called.");
        funFacts.Add("Snake", "Snakes smell with their tongues. Also, some snakes can go months without eating after a big meal.");
        funFacts.Add("Bear", "Bear can smell your lunch from 30 kilometers away. Also, some bears wave at people; they’ve learned it gets them snacks.");
        funFacts.Add("Chicken", "Chickens are basically tiny dinosaurs. Scientists say the chicken is the closest living relative of the T. rex dinosaur.");
        funFacts.Add("Cow", "Cows have best friends. They pick a buddy in the herd and become calmer when together (and can get stressed when apart).");
        funFacts.Add("Duck", "Ducks can sleep with one eye open. One half of their brain stays awake to watch for danger, while the other half rests.");
        funFacts.Add("Elephant","Elephants have an incredible sense of smell. They can detect water from approximately 19 km away, which helps them locate watering holes in the wild.");
        funFacts.Add("Fox","Fox can make over 40 sounds, from tiny chirps to spooky screams. Foxes also use their tails like blankets to keep warm while sleeping.");
        funFacts.Add("Frog","Frogs drink through their skin instead of their mouths. They can also jump over 20 times their body length in one leap.");
        funFacts.Add("Hedgehog","Hedgehogs are born with soft spines that harden within hours. And when scared, they roll into a tight ball to protect themselves.");
        funFacts.Add("Horse", "Horses can even sleep standing up. They have a special “stay” system in their leg ligaments that locks their knees, allowing them to doze on their feet without falling over.");
        funFacts.Add("Koala", "Has fingerprints like humans, so don’t let one near a cookie jar. Also, koalas sleep up to 20 hours a day, basically napping pros.");
        funFacts.Add("Lion", "The only cat that loves to live with friends in a big family called a pride. Also, a lion’s roar is so loud it can shake the ground up to 8 kilometers away.");
        funFacts.Add("Mouse", "Mice can sing — they make ultrasonic sounds to communicate. They are also excellent swimmers and jumpers, despite their size.");
        funFacts.Add("Owl", "Owls can turn their heads almost all the way around, up to 270°. They do this because their big eyes are fixed in their skulls, so they have to turn their necks to look around");
        funFacts.Add("Panda", "A panda eats bamboo all day, like 99% of its food is just sticks. Also, baby pandas are born tiny and pink, weighing less than a cup of tea.");
    }   

    // ✅ Clean Update: only handles time counting
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
        StartCoroutine(StartTimerWithDelay(0.1f));
    }

    // ✅ Only plays timer sound once with a 0.5s delay
    IEnumerator StartTimerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPlaying = true;

        yield return new WaitForSeconds(0.5f); // delay before sound

        if (timerAudioSource != null && timerSoundLoop != null)
        {
            timerAudioSource.clip = timerSoundLoop;
            timerAudioSource.loop = true;
            timerAudioSource.Play();
        }
    }

    public void PauseTimer() => isPlaying = false;
    public void ResumeTimer() => isPlaying = true;

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
                ShowFunFact(funFacts[animalName]);
        }
        else
{
    // 🔊 Play wrong pair sound
    if (audioSource != null && wrongPairSound != null)
        audioSource.PlayOneShot(wrongPairSound);

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
            yield return null;

        CheckLevelComplete();
    }

    void PlayAnimalSound(string animalName)
{
    if (audioSource == null) return;

    switch (animalName)
    {
        case "Cat":
            if (catSound != null) audioSource.PlayOneShot(catSound);
            break;
        case "Dog":
            if (dogSound != null) audioSource.PlayOneShot(dogSound);
            break;
        case "Pig":
            if (pigSound != null) audioSource.PlayOneShot(pigSound);
            break;
        case "Snake":
            if (snakeSound != null) audioSource.PlayOneShot(snakeSound);
            break;
        case "Bear":
            if (bearSound != null) audioSource.PlayOneShot(bearSound);
            break;
        case "Chicken":
            if (chickenSound != null) audioSource.PlayOneShot(chickenSound);
            break;
        case "Cow":
            if (cowSound != null) audioSource.PlayOneShot(cowSound);
            break;
        case "Duck":
            if (duckSound != null) audioSource.PlayOneShot(duckSound);
            break;
        case "Elephant":
            if (elephantSound != null) audioSource.PlayOneShot(elephantSound);
            break;
        case "Fox":
            if (foxSound != null) audioSource.PlayOneShot(foxSound);
            break;
        case "Frog":
            if (frogSound != null) audioSource.PlayOneShot(frogSound);
            break;
        case "Hedgehog":
            if (HedgehogSound != null) audioSource.PlayOneShot(HedgehogSound);
            break;
        case "Horse":
    if (horseSound != null) audioSource.PlayOneShot(horseSound);
    break;
case "Koala":
    if (koalaSound != null) audioSource.PlayOneShot(koalaSound);
    break;
case "Lion":
    if (lionSound != null) audioSource.PlayOneShot(lionSound);
    break;
case "Mouse":
    if (mouseSound != null) audioSource.PlayOneShot(mouseSound);
    break;
case "Owl":
    if (owlSound != null) audioSource.PlayOneShot(owlSound);
    break;
case "Panda":
    if (pandaSound != null) audioSource.PlayOneShot(pandaSound);
    break;
    default: Debug.LogWarning($"No sound found for animal: {animalName}"); break;
    }
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
            Debug.Log("🎉 Level Complete!");

            StopAllMusic();

            if (timerAudioSource != null && timerAudioSource.isPlaying)
                timerAudioSource.Stop();

            // 🔓 Unlock the next level
            UnlockNextLevel();
            PlayYouWinSound();
            StartCoroutine(ShowSummaryAfterDelay());
        }
    }

    void PlayYouWinSound()
    {
        if (youWinSound == null) return;

        AudioSource target = sfxAudioSource != null ? sfxAudioSource : audioSource;

        if (target != null)
            target.PlayOneShot(youWinSound);
        else
            AudioSource.PlayClipAtPoint(youWinSound, Camera.main.transform.position);
    }

    IEnumerator ShowSummaryAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (summaryUI != null)
        {
            summaryUI.SetActive(true);
            ShowSummaryResults();
        }
    }

    void ShowSummaryResults()
    {
        StartCoroutine(TypeSummaryResults());
    }

    Coroutine typingCoroutine;

    void ShowFunFact(string fact)
{
    if (funFactPanel != null && funFactText != null)
    {
        isPlaying = false;
        isShowingFunFact = true;
        SetCardInteractivity(false);
        funFactPanel.SetActive(true);

        // ⏸ Pause the timer sound
        if (timerAudioSource != null && timerAudioSource.isPlaying)
            timerAudioSource.Pause();

        // (Optional) Pause background music for focus
        if (bgMusicSource != null && bgMusicSource.isPlaying)
            bgMusicSource.Pause();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeFunFact(fact));
    }
}


    IEnumerator TypeFunFact(string fact)
    {
        funFactText.text = "";
        float typingSpeed = 0.03f;

        foreach (char letter in fact)
        {
            funFactText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(2.5f);
        StartCoroutine(HideFunFactAfterDelay());
    }

    IEnumerator HideFunFactAfterDelay()
{
    yield return new WaitForSeconds(0.3f);

    if (funFactPanel != null)
        funFactPanel.SetActive(false);

    // ✅ Resume game logic and sounds
    isPlaying = true;
    isShowingFunFact = false;
    SetCardInteractivity(true);

    // ▶️ Resume timer sound
    if (timerAudioSource != null)
        timerAudioSource.UnPause();

    // ▶️ Resume background music (optional)
    if (bgMusicSource != null)
        bgMusicSource.UnPause();
}


    public bool IsChecking() => isChecking;
    public void SetCardInteractivity(bool state) => canInteract = state;
    public bool CanInteract() => canInteract;

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

        foreach (char c in scoreString)
        {
            summaryScoreText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.8f);

        foreach (char c in timeString)
        {
            summaryTimeText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.8f);

        foreach (char c in attemptString)
        {
            summaryAttemptText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void StopAllMusic()
    {
        if (bgMusicSource != null && bgMusicSource.isPlaying)
            bgMusicSource.Stop();

        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            if (source != sfxAudioSource && source != audioSource)
            {
                if (source.isPlaying)
                    source.Stop();
            }
        }
    }

    public void PauseGameAndSound()
{
    // ⏸ Pause gameplay
    isPlaying = false;

    // Pause game time
    Time.timeScale = 0f;

    // Pause timer sound safely
    if (timerAudioSource != null && timerAudioSource.isPlaying)
        timerAudioSource.Pause();

    // Pause background music
    if (bgMusicSource != null && bgMusicSource.isPlaying)
        bgMusicSource.Pause();
}

public void ResumeGameAndSound()
{
    Time.timeScale = 1f;
    isPlaying = true;

    if (timerAudioSource != null)
        timerAudioSource.UnPause();

    if (bgMusicSource != null)
        bgMusicSource.UnPause();
}


private IEnumerator ResumeGameSmoothly()
{
    // Wait using real time (not affected by Time.timeScale)
    yield return new WaitForSecondsRealtime(0.1f);

    // Resume time and timer
    Time.timeScale = 1f;
    isPlaying = true;

    // ✅ Resume timer sound properly
    if (timerAudioSource != null)
    {
        // In case it was stopped or got reset by Unity, ensure it's looping
        if (!timerAudioSource.isPlaying)
        {
            timerAudioSource.clip = timerSoundLoop;
            timerAudioSource.loop = true;
            timerAudioSource.Play();
        }
        else
        {
            timerAudioSource.UnPause();
        }
    }

    // ✅ Resume background music
    if (bgMusicSource != null)
        bgMusicSource.UnPause();
}
    void UnlockNextLevel()
{
    string currentScene = SceneManager.GetActiveScene().name;

    // ✅ Extract the current level number safely
    if (currentScene.StartsWith("Level"))
    {
        string numberPart = currentScene.Replace("Level", "");
        if (int.TryParse(numberPart, out int currentLevel))
        {
            int nextLevel = currentLevel + 1;
            PlayerPrefs.SetInt($"LevelUnlocked_{nextLevel}", 1);
            PlayerPrefs.Save(); // 💾 make sure it’s written to disk
            Debug.Log($"✅ Level {nextLevel} unlocked!");
        }
        else
        {
            Debug.LogWarning("⚠️ Could not parse level number from scene name.");
        }
    }
    else
    {
        Debug.LogWarning("⚠️ Current scene is not named 'LevelX'.");
    }
}

}



