using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceLog : MonoBehaviour
{
    public Text titleText;
    public Text entriesText;
    public string emptyMessage = "No evidence recorded.";

    private readonly HashSet<string> discoveredIds = new HashSet<string>();
    private readonly List<EvidenceEntry> entries = new List<EvidenceEntry>();

    public event Action<string> EvidenceAdded;

    public bool AddEvidence(string evidenceId, string displayName, string description, bool requiredForObjective)
    {
        if (string.IsNullOrWhiteSpace(evidenceId))
        {
            evidenceId = displayName;
        }

        if (!discoveredIds.Add(evidenceId))
        {
            return false;
        }

        entries.Add(new EvidenceEntry(displayName, requiredForObjective));
        Refresh();

        if (EvidenceAdded != null)
        {
            EvidenceAdded.Invoke(evidenceId);
        }

        return true;
    }

    public bool HasEvidence(string evidenceId)
    {
        return !string.IsNullOrWhiteSpace(evidenceId) && discoveredIds.Contains(evidenceId);
    }

    public void ClearEvidence()
    {
        discoveredIds.Clear();
        entries.Clear();
        Refresh();
    }

    private void Awake()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (titleText != null)
        {
            titleText.text = "Evidence";
        }

        if (entriesText == null)
        {
            return;
        }

        if (entries.Count == 0)
        {
            entriesText.text = emptyMessage;
            return;
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < entries.Count; i++)
        {
            EvidenceEntry entry = entries[i];
            builder.Append("- ");
            builder.Append(entry.DisplayName);

            if (entry.RequiredForObjective)
            {
                builder.Append(" *");
            }

            if (i < entries.Count - 1)
            {
                builder.AppendLine();
            }
        }

        entriesText.text = builder.ToString();
    }

    private readonly struct EvidenceEntry
    {
        public readonly string DisplayName;
        public readonly bool RequiredForObjective;

        public EvidenceEntry(string displayName, bool requiredForObjective)
        {
            DisplayName = displayName;
            RequiredForObjective = requiredForObjective;
        }
    }
}
