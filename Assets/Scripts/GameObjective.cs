using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjective : MonoBehaviour
{
    public EvidenceLog evidenceLog;
    public Text objectiveText;
    public string incompleteText = "Objective: reconstruct what happened in the room.";
    public string completeText = "Objective complete: all essential evidence is logged.";

    [SerializeField]
    private List<string> requiredEvidenceIds = new List<string>();

    private bool complete;

    public bool IsComplete
    {
        get { return complete; }
    }

    public void Bind(EvidenceLog log, Text text)
    {
        if (evidenceLog != null)
        {
            evidenceLog.EvidenceAdded -= HandleEvidenceAdded;
        }

        evidenceLog = log;
        objectiveText = text;

        if (evidenceLog != null)
        {
            evidenceLog.EvidenceAdded += HandleEvidenceAdded;
        }

        Refresh();
    }

    public void SetRequiredEvidence(IEnumerable<string> ids)
    {
        requiredEvidenceIds.Clear();

        if (ids != null)
        {
            foreach (string id in ids)
            {
                if (!string.IsNullOrWhiteSpace(id) && !requiredEvidenceIds.Contains(id))
                {
                    requiredEvidenceIds.Add(id);
                }
            }
        }

        Refresh();
    }

    public void SetObjectiveComplete()
    {
        complete = true;
        if (objectiveText != null)
        {
            objectiveText.text = completeText;
        }
    }

    private void OnEnable()
    {
        if (evidenceLog != null)
        {
            evidenceLog.EvidenceAdded += HandleEvidenceAdded;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (evidenceLog != null)
        {
            evidenceLog.EvidenceAdded -= HandleEvidenceAdded;
        }
    }

    private void HandleEvidenceAdded(string evidenceId)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (evidenceLog == null || requiredEvidenceIds.Count == 0)
        {
            SetObjectiveText(incompleteText);
            return;
        }

        int discoveredRequired = 0;
        for (int i = 0; i < requiredEvidenceIds.Count; i++)
        {
            if (evidenceLog.HasEvidence(requiredEvidenceIds[i]))
            {
                discoveredRequired++;
            }
        }

        if (discoveredRequired >= requiredEvidenceIds.Count)
        {
            SetObjectiveComplete();
            return;
        }

        complete = false;
        SetObjectiveText(incompleteText + " " + discoveredRequired + "/" + requiredEvidenceIds.Count + " essential clues found.");
    }

    private void SetObjectiveText(string text)
    {
        if (objectiveText != null)
        {
            objectiveText.text = text;
        }
    }
}
