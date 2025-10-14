using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    List<CardController> revealedCards = new List<CardController>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupLevel1();
    }

    public void SetupLevel1()
    {
        if (animalSprites == null || animalSprites.Length < 2)
        {
            Debug.LogError("Assign 2 animal sprites for Level 1 in GameManager.");
            return;
        }

        List<Sprite> pool = new List<Sprite>();
        pool.Add(animalSprites[0]);
        pool.Add(animalSprites[0]);
        pool.Add(animalSprites[1]);
        pool.Add(animalSprites[1]);

        Shuffle(pool);

        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

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
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
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
        {
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.6f);

        if (revealedCards[0].frontSprite == revealedCards[1].frontSprite)
        {
            revealedCards[0].SetMatched();
            revealedCards[1].SetMatched();
        }
        else
        {
            StartCoroutine(revealedCards[0].FlipToBack());
            StartCoroutine(revealedCards[1].FlipToBack());
        }

        revealedCards.Clear();
        CheckLevelComplete();
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
            Debug.Log("Level Complete!");
            if (summaryUI != null)
                summaryUI.SetActive(true);
        }
    }
}
