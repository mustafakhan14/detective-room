using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    [Header("Scene References")]
    public Camera sceneCamera;
    public Text promptText;
    public InspectionPanel inspectionPanel;
    public DialoguePanel dialoguePanel;
    public EvidenceLog evidenceLog;

    [Header("Raycast")]
    public float rayDistance = 100f;

    private Interactable hovered;

    private void Update()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        UpdateHoveredInteractable();
        HandleClick();
    }

    public void Interact(Interactable interactable)
    {
        if (interactable == null)
        {
            return;
        }

        if (interactable.interactionType == InteractableType.Dialogue)
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.ShowDialogue(interactable.displayName, interactable.dialogueLines);
            }

            return;
        }

        if (inspectionPanel != null)
        {
            inspectionPanel.ShowInspection(interactable.displayName, interactable.description);
        }

        if (evidenceLog != null)
        {
            evidenceLog.AddEvidence(
                interactable.evidenceId,
                interactable.displayName,
                interactable.description,
                interactable.requiredForObjective);
        }
    }

    private void UpdateHoveredInteractable()
    {
        Interactable nextHover = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()
            ? null
            : FindInteractableUnderCursor();

        if (hovered == nextHover)
        {
            return;
        }

        if (hovered != null)
        {
            hovered.SetHighlighted(false);
        }

        hovered = nextHover;

        if (hovered != null)
        {
            hovered.SetHighlighted(true);
            SetPrompt(hovered.GetPrompt());
        }
        else
        {
            SetPrompt("Click the floor to move. Hover a clue to inspect it.");
        }
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (hovered != null)
        {
            Interact(hovered);
        }
    }

    private Interactable FindInteractableUnderCursor()
    {
        if (sceneCamera == null)
        {
            return null;
        }

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);
        if (hits.Length == 0)
        {
            return null;
        }

        Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            Interactable interactable = hits[i].collider.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                return interactable;
            }
        }

        return null;
    }

    private void SetPrompt(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
    }

    private void OnDisable()
    {
        if (hovered != null)
        {
            hovered.SetHighlighted(false);
            hovered = null;
        }
    }
}
