using UnityEngine;

public enum InteractableType
{
    Clue,
    Dialogue
}

[DisallowMultipleComponent]
public class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    public string displayName = "Inspectable";
    [TextArea(3, 8)]
    public string description = "There is something worth noticing here.";
    public string evidenceId = "evidence_id";
    public bool requiredForObjective = true;
    public InteractableType interactionType = InteractableType.Clue;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] dialogueLines;

    [Header("Feedback")]
    public Color highlightColor = new Color(1f, 0.78f, 0.28f, 1f);

    private Renderer[] cachedRenderers;
    private bool highlighted;

    private void Awake()
    {
        CacheRenderers();
    }

    public string GetPrompt()
    {
        string verb = interactionType == InteractableType.Dialogue ? "Talk" : "Inspect";
        return verb + ": " + displayName;
    }

    public void SetHighlighted(bool value)
    {
        if (highlighted == value)
        {
            return;
        }

        highlighted = value;
        CacheRenderers();

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            Renderer targetRenderer = cachedRenderers[i];
            if (targetRenderer == null)
            {
                continue;
            }

            if (!highlighted)
            {
                targetRenderer.SetPropertyBlock(null);
                continue;
            }

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(block);
            block.SetColor("_Color", highlightColor);
            block.SetColor("_BaseColor", highlightColor);
            block.SetColor("_EmissionColor", highlightColor * 0.35f);
            targetRenderer.SetPropertyBlock(block);
        }
    }

    private void CacheRenderers()
    {
        if (cachedRenderers == null || cachedRenderers.Length == 0)
        {
            cachedRenderers = GetComponentsInChildren<Renderer>();
        }
    }

    private void OnDisable()
    {
        SetHighlighted(false);
    }
}
