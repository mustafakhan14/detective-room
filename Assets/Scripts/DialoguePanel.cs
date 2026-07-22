using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public GameObject panelRoot;
    public Text speakerText;
    public Text bodyText;
    public Text nextButtonText;
    public Button nextButton;
    public Button closeButton;

    private string[] activeLines = new string[0];
    private int lineIndex;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(Advance);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        Close();
    }

    public void ShowDialogue(string speaker, string[] dialogueLines)
    {
        activeLines = dialogueLines != null && dialogueLines.Length > 0
            ? dialogueLines
            : new[] { "No one answers, but the silence has a shape." };
        lineIndex = 0;

        if (speakerText != null)
        {
            speakerText.text = speaker;
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }

        RefreshLine();
    }

    public void Advance()
    {
        if (activeLines == null || activeLines.Length == 0)
        {
            Close();
            return;
        }

        if (lineIndex >= activeLines.Length - 1)
        {
            Close();
            return;
        }

        lineIndex++;
        RefreshLine();
    }

    public void Close()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void RefreshLine()
    {
        if (bodyText != null && activeLines != null && activeLines.Length > 0)
        {
            bodyText.text = activeLines[Mathf.Clamp(lineIndex, 0, activeLines.Length - 1)];
        }

        if (nextButtonText != null)
        {
            nextButtonText.text = lineIndex >= activeLines.Length - 1 ? "Close" : "Next";
        }
    }
}
