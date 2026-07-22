using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class CoreBehaviourTests
{
    private GameObject owner;

    [TearDown]
    public void TearDown()
    {
        if (owner != null)
        {
            Object.DestroyImmediate(owner);
        }
    }

    [Test]
    public void EvidenceLogRecordsUniqueEvidenceAndUpdatesText()
    {
        owner = new GameObject("Evidence Log Test");
        Text titleText = CreateText("Title");
        Text entriesText = CreateText("Entries");
        EvidenceLog evidenceLog = owner.AddComponent<EvidenceLog>();
        evidenceLog.titleText = titleText;
        evidenceLog.entriesText = entriesText;

        bool firstAdd = evidenceLog.AddEvidence("broken_glass", "Broken Glass", "Sharp clue.", true);
        bool duplicateAdd = evidenceLog.AddEvidence("broken_glass", "Broken Glass", "Sharp clue.", true);

        Assert.IsTrue(firstAdd);
        Assert.IsFalse(duplicateAdd);
        Assert.IsTrue(evidenceLog.HasEvidence("broken_glass"));
        Assert.AreEqual("Evidence", titleText.text);
        Assert.AreEqual("- Broken Glass *", entriesText.text);
    }

    [Test]
    public void GameObjectiveCompletesAfterAllRequiredEvidence()
    {
        owner = new GameObject("Objective Test");
        Text objectiveText = CreateText("Objective Text");
        EvidenceLog evidenceLog = owner.AddComponent<EvidenceLog>();
        GameObjective objective = owner.AddComponent<GameObjective>();
        objective.Bind(evidenceLog, objectiveText);
        objective.SetRequiredEvidence(new[] { "broken_glass", "ledger" });

        Assert.IsFalse(objective.IsComplete);
        StringAssert.Contains("0/2 essential clues found", objectiveText.text);

        evidenceLog.AddEvidence("broken_glass", "Broken Glass", "Sharp clue.", true);

        Assert.IsFalse(objective.IsComplete);
        StringAssert.Contains("1/2 essential clues found", objectiveText.text);

        evidenceLog.AddEvidence("ledger", "Ledger", "Payment clue.", true);

        Assert.IsTrue(objective.IsComplete);
        Assert.AreEqual(objective.completeText, objectiveText.text);
    }

    [Test]
    public void InteractablePromptMatchesInteractionType()
    {
        owner = new GameObject("Interactable Prompt Test");
        Interactable clue = owner.AddComponent<Interactable>();
        clue.displayName = "Ledger";
        clue.interactionType = InteractableType.Clue;

        Assert.AreEqual("Inspect: Ledger", clue.GetPrompt());

        clue.displayName = "Radio Dispatcher";
        clue.interactionType = InteractableType.Dialogue;

        Assert.AreEqual("Talk: Radio Dispatcher", clue.GetPrompt());
    }

    [Test]
    public void InspectionPanelShowsAndClosesAssignedContent()
    {
        owner = new GameObject("Inspection Panel Test");
        GameObject panelRoot = CreateChild("Inspection Root");
        Text titleText = CreateText("Inspection Title");
        Text bodyText = CreateText("Inspection Body");
        InspectionPanel panel = owner.AddComponent<InspectionPanel>();
        panel.panelRoot = panelRoot;
        panel.titleText = titleText;
        panel.bodyText = bodyText;

        panel.ShowInspection("Locked Door", "The lock is scratched.");

        Assert.IsTrue(panelRoot.activeSelf);
        Assert.AreEqual("Locked Door", titleText.text);
        Assert.AreEqual("The lock is scratched.", bodyText.text);

        panel.Close();

        Assert.IsFalse(panelRoot.activeSelf);
    }

    [Test]
    public void DialoguePanelAdvancesThenClosesOnFinalLine()
    {
        owner = new GameObject("Dialogue Panel Test");
        GameObject panelRoot = CreateChild("Dialogue Root");
        Text speakerText = CreateText("Speaker");
        Text bodyText = CreateText("Dialogue Body");
        Text nextButtonText = CreateText("Next Button Text");
        DialoguePanel panel = owner.AddComponent<DialoguePanel>();
        panel.panelRoot = panelRoot;
        panel.speakerText = speakerText;
        panel.bodyText = bodyText;
        panel.nextButtonText = nextButtonText;

        panel.ShowDialogue("Radio Dispatcher", new[] { "Line one.", "Line two." });

        Assert.IsTrue(panelRoot.activeSelf);
        Assert.AreEqual("Radio Dispatcher", speakerText.text);
        Assert.AreEqual("Line one.", bodyText.text);
        Assert.AreEqual("Next", nextButtonText.text);

        panel.Advance();

        Assert.IsTrue(panelRoot.activeSelf);
        Assert.AreEqual("Line two.", bodyText.text);
        Assert.AreEqual("Close", nextButtonText.text);

        panel.Advance();

        Assert.IsFalse(panelRoot.activeSelf);
    }

    [Test]
    public void InteractionControllerInspectsCluesAndLogsEvidence()
    {
        owner = new GameObject("Interaction Controller Clue Test");
        Text logTitle = CreateText("Log Title");
        Text logEntries = CreateText("Log Entries");
        Text inspectionTitle = CreateText("Inspection Title");
        Text inspectionBody = CreateText("Inspection Body");
        GameObject inspectionRoot = CreateChild("Inspection Root");

        EvidenceLog evidenceLog = owner.AddComponent<EvidenceLog>();
        evidenceLog.titleText = logTitle;
        evidenceLog.entriesText = logEntries;

        InspectionPanel inspectionPanel = owner.AddComponent<InspectionPanel>();
        inspectionPanel.panelRoot = inspectionRoot;
        inspectionPanel.titleText = inspectionTitle;
        inspectionPanel.bodyText = inspectionBody;

        InteractionController controller = owner.AddComponent<InteractionController>();
        controller.evidenceLog = evidenceLog;
        controller.inspectionPanel = inspectionPanel;

        Interactable clue = CreateChild("Ledger").AddComponent<Interactable>();
        clue.displayName = "Ledger";
        clue.description = "A circled midnight payment.";
        clue.evidenceId = "ledger";
        clue.requiredForObjective = true;

        controller.Interact(clue);

        Assert.IsTrue(inspectionRoot.activeSelf);
        Assert.AreEqual("Ledger", inspectionTitle.text);
        Assert.AreEqual("A circled midnight payment.", inspectionBody.text);
        Assert.IsTrue(evidenceLog.HasEvidence("ledger"));
        Assert.AreEqual("- Ledger *", logEntries.text);
    }

    [Test]
    public void InteractionControllerStartsDialogueWithoutLoggingEvidence()
    {
        owner = new GameObject("Interaction Controller Dialogue Test");
        Text logEntries = CreateText("Log Entries");
        Text speakerText = CreateText("Speaker");
        Text bodyText = CreateText("Dialogue Body");
        GameObject dialogueRoot = CreateChild("Dialogue Root");

        EvidenceLog evidenceLog = owner.AddComponent<EvidenceLog>();
        evidenceLog.entriesText = logEntries;

        DialoguePanel dialoguePanel = owner.AddComponent<DialoguePanel>();
        dialoguePanel.panelRoot = dialogueRoot;
        dialoguePanel.speakerText = speakerText;
        dialoguePanel.bodyText = bodyText;

        InteractionController controller = owner.AddComponent<InteractionController>();
        controller.evidenceLog = evidenceLog;
        controller.dialoguePanel = dialoguePanel;

        Interactable radio = CreateChild("Radio").AddComponent<Interactable>();
        radio.displayName = "Radio Dispatcher";
        radio.evidenceId = "radio_dispatcher";
        radio.interactionType = InteractableType.Dialogue;
        radio.dialogueLines = new[] { "Still there, detective?" };

        controller.Interact(radio);

        Assert.IsTrue(dialogueRoot.activeSelf);
        Assert.AreEqual("Radio Dispatcher", speakerText.text);
        Assert.AreEqual("Still there, detective?", bodyText.text);
        Assert.IsFalse(evidenceLog.HasEvidence("radio_dispatcher"));
    }

    [Test]
    public void PlayerClickMoverMoveToClampsTargetMarkerToRoomBounds()
    {
        owner = new GameObject("Player Click Mover Test");
        GameObject marker = CreateChild("Target Marker");
        owner.AddComponent<BoxCollider>();
        PlayerClickMover mover = owner.AddComponent<PlayerClickMover>();
        mover.roomBounds = new Vector2(2f, 1f);
        mover.targetMarker = marker.transform;

        mover.MoveTo(new Vector3(20f, 99f, -20f));

        Assert.IsTrue(marker.activeSelf);
        Assert.AreEqual(new Vector3(2f, 0.03f, -1f), marker.transform.position);
    }

    private Text CreateText(string name)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(owner.transform, false);
        return textObject.AddComponent<Text>();
    }

    private GameObject CreateChild(string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(owner.transform, false);
        return child;
    }
}
