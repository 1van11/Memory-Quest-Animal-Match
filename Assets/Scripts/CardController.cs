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
        // 🚫 Stop if game manager not ready
        if (GameManager.instance == null) return;

        // 🚫 Block if checking pairs OR fun fact showing OR interactivity off
        if (GameManager.instance.IsChecking()) return;
        if (!GameManager.instance.CanInteract()) return;

        // 🚫 Skip if matched, flipping, or already flipped
        if (isMatched || isAnimating || isFlipped) return;

        // 🎵 ✅ Play flip sound (with slight random pitch variation)
        if (GameManager.instance.sfxAudioSource != null && GameManager.instance.flipCardSound != null)
        {
            GameManager.instance.sfxAudioSource.pitch = Random.Range(0.95f, 1.05f); // adds variation
            GameManager.instance.sfxAudioSource.PlayOneShot(GameManager.instance.flipCardSound);
        }

        // ✅ Flip and notify GameManager
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
        float duration = 0.4f; // slightly slower for smoothness
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t); // Smoothstep curve
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, endAngle, 0);

        // Optional bounce (tiny wobble effect after flip)
        if (endAngle == 0f)
        {
            StartCoroutine(BounceEffect());
        }
    }

    IEnumerator BounceEffect()
    {
        float bounceTime = 0.15f;
        float bounceAmount = 1.05f; // 5% bigger scale bounce

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * bounceAmount;

        float t = 0f;
        while (t < bounceTime)
        {
            float scale = Mathf.Lerp(1f, bounceAmount, t / bounceTime);
            transform.localScale = originalScale * scale;
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < bounceTime)
        {
            float scale = Mathf.Lerp(bounceAmount, 1f, t / bounceTime);
            transform.localScale = originalScale * scale;
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
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
