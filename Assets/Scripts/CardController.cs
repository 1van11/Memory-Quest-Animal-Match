using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardController : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite backSprite;

    private Image image;
    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isAnimating = false;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        ShowBackInstant();
    }

    public void OnClick()
    {
        // ❌ Stop if game manager isn't ready or we're mid-check
        if (GameManager.instance == null) return;
        if (GameManager.instance.IsChecking()) return; // <== NEW
        if (isMatched || isAnimating || isFlipped) return;

        StartCoroutine(FlipToFront());
        GameManager.instance.CardRevealed(this);
    }

    public IEnumerator FlipToFront()
    {
        isAnimating = true;
        yield return StartCoroutine(Flip(0f, 90f));
        image.sprite = frontSprite;
        yield return StartCoroutine(Flip(90f, 0f));
        isFlipped = true;
        isAnimating = false;
    }

    public IEnumerator FlipToBack()
    {
        isAnimating = true;
        yield return StartCoroutine(Flip(0f, 90f));
        image.sprite = backSprite;
        yield return StartCoroutine(Flip(90f, 0f));
        isFlipped = false;
        isAnimating = false;
    }

    IEnumerator Flip(float startAngle, float endAngle)
    {
        float duration = 0.25f;
        float time = 0f;

        while (time < duration)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, time / duration);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, endAngle, 0);
    }

    public void ShowBackInstant()
    {
        if (backSprite != null)
            image.sprite = backSprite;
        transform.localRotation = Quaternion.identity;
        isFlipped = false;
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public bool IsMatched() => isMatched;
}
