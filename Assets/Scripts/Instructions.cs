using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    [Header("UI References")]
    public GameObject instructionsPanel;
    public TextMeshProUGUI instructionsText;
    public Button instructionsButton;
    public Button closeButton;

    [Header("Typing Settings")]
    [TextArea]
    public string instructionContent = "";
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;

    void Start()
    {
        // Hide panel on start
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        // Hook up buttons
        if (instructionsButton != null)
            instructionsButton.onClick.AddListener(ShowInstructions);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideInstructions);
    }

    public void ShowInstructions()
    {
        if (instructionsPanel == null || instructionsText == null) return;

        instructionsPanel.SetActive(true);
        instructionsText.text = ""; // Clear text before typing starts

        // Start typing animation
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char c in instructionContent)
        {
            instructionsText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
    }
}
