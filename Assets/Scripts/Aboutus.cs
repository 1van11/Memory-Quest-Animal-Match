using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Aboutus : MonoBehaviour
{
    [Header("UI References")]
    public GameObject aboutUsPanel;
    public TextMeshProUGUI aboutUsText;
    public Button aboutButton;
    public Button closeButton;

    [Header("Typing Settings")]
    [TextArea] public string aboutText = "";
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;

    void Start()
    {
        // Hide panel on start
        if (aboutUsPanel != null)
            aboutUsPanel.SetActive(false);

        // Hook up buttons
        if (aboutButton != null)
            aboutButton.onClick.AddListener(ShowAboutUs);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideAboutUs);
    }

    public void ShowAboutUs()
    {
        if (aboutUsPanel == null || aboutUsText == null) return;

        aboutUsPanel.SetActive(true);
        aboutUsText.text = ""; // Clear text before typing starts

        // Start typing animation
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char c in aboutText)
        {
            aboutUsText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    public void HideAboutUs()
    {
        if (aboutUsPanel != null)
            aboutUsPanel.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
    }
}
