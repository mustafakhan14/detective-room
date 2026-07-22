using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class DetectiveRoomBootstrap : MonoBehaviour
{
    public bool buildInEditMode = true;
    public bool rebuildWhenPlaying = true;
    public Vector2 roomBounds = new Vector2(4.6f, 3.1f);

    private const string GeneratedRootName = "__DetectiveRoomGenerated";

    private void OnEnable()
    {
        if (!Application.isPlaying && buildInEditMode)
        {
            EnsureBuilt(false);
        }
    }

    private void Awake()
    {
        if (Application.isPlaying)
        {
            EnsureBuilt(rebuildWhenPlaying);
        }
    }

    [ContextMenu("Rebuild Detective Room")]
    public void RebuildDetectiveRoom()
    {
        EnsureBuilt(true);
    }

    private void EnsureBuilt(bool forceRebuild)
    {
        Transform existing = transform.Find(GeneratedRootName);
        if (existing != null && !forceRebuild)
        {
            return;
        }

        if (existing != null)
        {
            existing.gameObject.SetActive(false);
            DestroyGenerated(existing.gameObject);
        }

        BuildRoom();
    }

    private void BuildRoom()
    {
        GameObject root = new GameObject(GeneratedRootName);
        root.transform.SetParent(transform, false);

        Dictionary<string, Material> materials = CreateMaterials();
        Camera sceneCamera = CreateCamera(root.transform);

        CreateLighting(root.transform);
        CreateRoomShell(root.transform, materials);

        List<string> requiredEvidenceIds = new List<string>();
        CreateFurnitureAndClues(root.transform, materials, requiredEvidenceIds);
        CreatePlayer(root.transform, materials, sceneCamera);

        BuildUi(
            root.transform,
            out Text promptText,
            out EvidenceLog evidenceLog,
            out InspectionPanel inspectionPanel,
            out DialoguePanel dialoguePanel,
            out Text objectiveText);

        InteractionController interactionController = root.AddComponent<InteractionController>();
        interactionController.sceneCamera = sceneCamera;
        interactionController.promptText = promptText;
        interactionController.inspectionPanel = inspectionPanel;
        interactionController.dialoguePanel = dialoguePanel;
        interactionController.evidenceLog = evidenceLog;

        GameObjective objective = root.AddComponent<GameObjective>();
        objective.Bind(evidenceLog, objectiveText);
        objective.SetRequiredEvidence(requiredEvidenceIds);
    }

    private Dictionary<string, Material> CreateMaterials()
    {
        Dictionary<string, Material> materials = new Dictionary<string, Material>();
        materials["floor"] = CreateMaterial("Scuffed Walnut Floor", new Color(0.27f, 0.20f, 0.15f), 0.05f, 0.35f);
        materials["wall"] = CreateMaterial("Faded Green Wall", new Color(0.28f, 0.36f, 0.32f), 0f, 0.25f);
        materials["trim"] = CreateMaterial("Dark Wood Trim", new Color(0.12f, 0.08f, 0.055f), 0f, 0.3f);
        materials["desk"] = CreateMaterial("Desk Wood", new Color(0.24f, 0.12f, 0.07f), 0f, 0.4f);
        materials["paper"] = CreateMaterial("Case Paper", new Color(0.78f, 0.72f, 0.58f), 0f, 0.15f);
        materials["ledger"] = CreateMaterial("Red Ledger", new Color(0.38f, 0.05f, 0.08f), 0f, 0.32f);
        materials["glass"] = CreateMaterial("Cold Glass", new Color(0.6f, 0.86f, 0.95f, 0.7f), 0f, 0.05f);
        materials["metal"] = CreateMaterial("Dull Metal", new Color(0.33f, 0.34f, 0.34f), 0.1f, 0.45f);
        materials["door"] = CreateMaterial("Locked Door", new Color(0.16f, 0.11f, 0.075f), 0f, 0.28f);
        materials["player"] = CreateMaterial("Detective Coat", new Color(0.11f, 0.16f, 0.20f), 0f, 0.38f);
        materials["accent"] = CreateMaterial("Evidence Amber", new Color(0.95f, 0.67f, 0.25f), 0f, 0.2f);
        materials["outline"] = CreateMaterial("Incident Chalk", new Color(0.86f, 0.82f, 0.72f), 0f, 0.1f);
        materials["shadow"] = CreateMaterial("Deep Shadow", new Color(0.035f, 0.04f, 0.045f), 0f, 0.5f);
        materials["window"] = CreateMaterial("Rain Window", new Color(0.22f, 0.36f, 0.46f), 0f, 0.08f);
        return materials;
    }

    private Camera CreateCamera(Transform parent)
    {
        GameObject cameraObject = new GameObject("2.5D Orthographic Camera");
        cameraObject.transform.SetParent(parent, false);
        cameraObject.transform.position = new Vector3(5.9f, 6.4f, -6.2f);
        cameraObject.transform.rotation = Quaternion.Euler(55f, -42f, 0f);
        cameraObject.tag = "MainCamera";

        Camera sceneCamera = cameraObject.AddComponent<Camera>();
        sceneCamera.orthographic = true;
        sceneCamera.orthographicSize = 5.15f;
        sceneCamera.nearClipPlane = 0.1f;
        sceneCamera.farClipPlane = 100f;
        sceneCamera.backgroundColor = new Color(0.035f, 0.043f, 0.055f);
        cameraObject.AddComponent<AudioListener>();

        return sceneCamera;
    }

    private void CreateLighting(Transform parent)
    {
        GameObject keyObject = new GameObject("Cold Window Key Light");
        keyObject.transform.SetParent(parent, false);
        keyObject.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        Light key = keyObject.AddComponent<Light>();
        key.type = LightType.Directional;
        key.intensity = 0.7f;
        key.color = new Color(0.62f, 0.78f, 1f);
        key.shadows = LightShadows.Soft;

        GameObject lampObject = new GameObject("Desk Lamp Glow");
        lampObject.transform.SetParent(parent, false);
        lampObject.transform.position = new Vector3(-2.2f, 1.8f, 0.55f);
        Light lamp = lampObject.AddComponent<Light>();
        lamp.type = LightType.Point;
        lamp.intensity = 2.7f;
        lamp.range = 4.2f;
        lamp.color = new Color(1f, 0.72f, 0.42f);
        lamp.shadows = LightShadows.Soft;
    }

    private void CreateRoomShell(Transform parent, Dictionary<string, Material> materials)
    {
        CreateCube("Floor", parent, new Vector3(0f, -0.05f, 0f), new Vector3(10f, 0.1f, 7f), materials["floor"]);
        CreateCube("Back Wall", parent, new Vector3(0f, 1.5f, 3.55f), new Vector3(10f, 3f, 0.1f), materials["wall"]);
        CreateCube("Left Wall", parent, new Vector3(-5.05f, 1.5f, 0f), new Vector3(0.1f, 3f, 7f), materials["wall"]);
        CreateCube("Right Wall Return", parent, new Vector3(5.05f, 1.5f, 1.25f), new Vector3(0.1f, 3f, 4.5f), materials["wall"]);
        CreateCube("Baseboard Back", parent, new Vector3(0f, 0.15f, 3.48f), new Vector3(10f, 0.18f, 0.12f), materials["trim"]);
        CreateCube("Baseboard Left", parent, new Vector3(-4.98f, 0.15f, 0f), new Vector3(0.12f, 0.18f, 7f), materials["trim"]);
        CreateCube("Baseboard Right", parent, new Vector3(4.98f, 0.15f, 1.25f), new Vector3(0.12f, 0.18f, 4.5f), materials["trim"]);

        GameObject window = CreateCube("Rain Streaked Window", parent, new Vector3(-1.2f, 1.9f, 3.47f), new Vector3(2.6f, 0.95f, 0.04f), materials["window"]);
        RemoveCollider(window);
        CreateCube("Window Crossbar H", parent, new Vector3(-1.2f, 1.9f, 3.43f), new Vector3(2.7f, 0.08f, 0.06f), materials["trim"]);
        CreateCube("Window Crossbar V", parent, new Vector3(-1.2f, 1.9f, 3.42f), new Vector3(0.08f, 1.05f, 0.06f), materials["trim"]);
    }

    private void CreateFurnitureAndClues(Transform parent, Dictionary<string, Material> materials, List<string> requiredEvidenceIds)
    {
        CreateDesk(parent, materials);
        CreateChair(parent, materials);
        CreateCabinet(parent, materials);
        CreateIncidentMarker(parent, materials);

        Interactable brokenGlass = CreateBrokenGlass(parent, materials);
        requiredEvidenceIds.Add(brokenGlass.evidenceId);

        GameObject ledger = CreateCube("Ledger", parent, new Vector3(-1.75f, 0.93f, 0.72f), new Vector3(0.65f, 0.07f, 0.45f), materials["ledger"]);
        ledger.transform.rotation = Quaternion.Euler(0f, -13f, 0f);
        Interactable ledgerInteractable = AddInteractable(
            ledger,
            "Ledger",
            "ledger",
            "A wine-dark ledger lies open to a page of cash payments. One entry is circled twice: room 3, midnight, no receipt.",
            true,
            InteractableType.Clue);
        requiredEvidenceIds.Add(ledgerInteractable.evidenceId);

        GameObject door = CreateCube("Locked Door", parent, new Vector3(5.0f, 1.05f, -1.75f), new Vector3(0.12f, 2.1f, 1.15f), materials["door"]);
        Interactable doorInteractable = AddInteractable(
            door,
            "Locked Door",
            "locked_door",
            "The lock is scratched around the keyway. Someone worked quickly and did not care about being neat.",
            true,
            InteractableType.Clue);
        requiredEvidenceIds.Add(doorInteractable.evidenceId);

        CreateCube("Door Handle", parent, new Vector3(4.9f, 1.05f, -1.32f), new Vector3(0.16f, 0.12f, 0.12f), materials["metal"]);
        CreateRadio(parent, materials);
    }

    private void CreateDesk(Transform parent, Dictionary<string, Material> materials)
    {
        CreateCube("Desk Top", parent, new Vector3(-1.7f, 0.75f, 0.7f), new Vector3(2.25f, 0.18f, 1.05f), materials["desk"]);
        CreateCube("Desk Left Leg", parent, new Vector3(-2.65f, 0.35f, 0.25f), new Vector3(0.18f, 0.7f, 0.18f), materials["desk"]);
        CreateCube("Desk Right Leg", parent, new Vector3(-0.75f, 0.35f, 0.25f), new Vector3(0.18f, 0.7f, 0.18f), materials["desk"]);
        CreateCube("Desk Back Leg", parent, new Vector3(-0.75f, 0.35f, 1.15f), new Vector3(0.18f, 0.7f, 0.18f), materials["desk"]);
        CreateCube("Desk Drawer", parent, new Vector3(-1.65f, 0.57f, 0.16f), new Vector3(1.1f, 0.28f, 0.08f), materials["trim"]);
        CreateCube("Loose Case Paper", parent, new Vector3(-2.35f, 0.88f, 0.78f), new Vector3(0.46f, 0.025f, 0.33f), materials["paper"]);

        GameObject lampBase = CreatePrimitive(PrimitiveType.Cylinder, "Desk Lamp Base", parent, new Vector3(-2.25f, 0.9f, 0.35f), new Vector3(0.22f, 0.05f, 0.22f), materials["metal"]);
        RemoveCollider(lampBase);
        GameObject lampShade = CreatePrimitive(PrimitiveType.Cylinder, "Desk Lamp Shade", parent, new Vector3(-2.25f, 1.22f, 0.35f), new Vector3(0.28f, 0.14f, 0.28f), materials["accent"]);
        RemoveCollider(lampShade);
        CreateCube("Desk Lamp Stem", parent, new Vector3(-2.25f, 1.07f, 0.35f), new Vector3(0.05f, 0.34f, 0.05f), materials["metal"]);
    }

    private void CreateChair(Transform parent, Dictionary<string, Material> materials)
    {
        CreateCube("Chair Seat", parent, new Vector3(-1.1f, 0.42f, -0.55f), new Vector3(0.75f, 0.15f, 0.75f), materials["desk"]);
        CreateCube("Chair Back", parent, new Vector3(-1.1f, 0.9f, -0.92f), new Vector3(0.75f, 0.9f, 0.13f), materials["desk"]);
        CreateCube("Chair Front Left Leg", parent, new Vector3(-1.42f, 0.2f, -0.25f), new Vector3(0.12f, 0.4f, 0.12f), materials["desk"]);
        CreateCube("Chair Front Right Leg", parent, new Vector3(-0.78f, 0.2f, -0.25f), new Vector3(0.12f, 0.4f, 0.12f), materials["desk"]);
    }

    private void CreateCabinet(Transform parent, Dictionary<string, Material> materials)
    {
        CreateCube("Evidence Cabinet", parent, new Vector3(2.85f, 0.75f, 2.85f), new Vector3(1.25f, 1.5f, 0.42f), materials["trim"]);
        CreateCube("Cabinet Door Left", parent, new Vector3(2.55f, 0.78f, 2.61f), new Vector3(0.48f, 1.18f, 0.05f), materials["desk"]);
        CreateCube("Cabinet Door Right", parent, new Vector3(3.15f, 0.78f, 2.61f), new Vector3(0.48f, 1.18f, 0.05f), materials["desk"]);
        CreateCube("Cabinet Handle Left", parent, new Vector3(2.72f, 0.82f, 2.55f), new Vector3(0.05f, 0.28f, 0.05f), materials["metal"]);
        CreateCube("Cabinet Handle Right", parent, new Vector3(2.98f, 0.82f, 2.55f), new Vector3(0.05f, 0.28f, 0.05f), materials["metal"]);
    }

    private void CreateIncidentMarker(Transform parent, Dictionary<string, Material> materials)
    {
        GameObject markerRoot = new GameObject("Incident Marker");
        markerRoot.transform.SetParent(parent, false);
        markerRoot.transform.localPosition = new Vector3(1.15f, 0.02f, -0.85f);
        markerRoot.transform.localRotation = Quaternion.Euler(0f, -18f, 0f);

        CreateCube("Chalk Spine", markerRoot.transform, new Vector3(0f, 0f, 0f), new Vector3(0.12f, 0.025f, 1.45f), materials["outline"]);
        CreateCube("Chalk Shoulders", markerRoot.transform, new Vector3(0f, 0f, 0.42f), new Vector3(0.95f, 0.025f, 0.12f), materials["outline"]);
        CreateCube("Chalk Left Arm", markerRoot.transform, new Vector3(-0.54f, 0f, 0.1f), new Vector3(0.12f, 0.025f, 0.85f), materials["outline"]);
        CreateCube("Chalk Right Arm", markerRoot.transform, new Vector3(0.54f, 0f, 0.1f), new Vector3(0.12f, 0.025f, 0.85f), materials["outline"]);
        CreateCube("Chalk Foot Line", markerRoot.transform, new Vector3(0f, 0f, -0.76f), new Vector3(0.75f, 0.025f, 0.12f), materials["outline"]);
    }

    private Interactable CreateBrokenGlass(Transform parent, Dictionary<string, Material> materials)
    {
        GameObject glassRoot = new GameObject("Broken Glass");
        glassRoot.transform.SetParent(parent, false);
        glassRoot.transform.localPosition = new Vector3(0.55f, 0.05f, 1.25f);

        GameObject shardA = CreateCube("Glass Shard A", glassRoot.transform, new Vector3(-0.24f, 0f, 0.04f), new Vector3(0.32f, 0.025f, 0.08f), materials["glass"]);
        shardA.transform.localRotation = Quaternion.Euler(0f, 28f, 0f);
        GameObject shardB = CreateCube("Glass Shard B", glassRoot.transform, new Vector3(0.05f, 0f, -0.08f), new Vector3(0.22f, 0.025f, 0.07f), materials["glass"]);
        shardB.transform.localRotation = Quaternion.Euler(0f, -18f, 0f);
        GameObject shardC = CreateCube("Glass Shard C", glassRoot.transform, new Vector3(0.28f, 0f, 0.13f), new Vector3(0.26f, 0.025f, 0.09f), materials["glass"]);
        shardC.transform.localRotation = Quaternion.Euler(0f, 63f, 0f);

        BoxCollider collider = glassRoot.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.06f, 0f);
        collider.size = new Vector3(0.9f, 0.18f, 0.55f);

        return AddInteractable(
            glassRoot,
            "Broken Glass",
            "broken_glass",
            "The shards are inside the room, spread away from the window. The break came from outside pressure, not a thrown bottle.",
            true,
            InteractableType.Clue);
    }

    private void CreateRadio(Transform parent, Dictionary<string, Material> materials)
    {
        GameObject radioRoot = new GameObject("Radio Dispatcher");
        radioRoot.transform.SetParent(parent, false);
        radioRoot.transform.localPosition = new Vector3(-1.05f, 0.93f, 0.42f);

        CreateCube("Radio Body", radioRoot.transform, new Vector3(0f, 0f, 0f), new Vector3(0.45f, 0.24f, 0.32f), materials["metal"]);
        CreateCube("Radio Speaker", radioRoot.transform, new Vector3(-0.08f, 0.03f, -0.18f), new Vector3(0.2f, 0.12f, 0.04f), materials["shadow"]);
        GameObject antenna = CreateCube("Radio Antenna", radioRoot.transform, new Vector3(0.22f, 0.28f, 0.05f), new Vector3(0.035f, 0.5f, 0.035f), materials["metal"]);
        antenna.transform.localRotation = Quaternion.Euler(0f, 0f, -20f);

        BoxCollider collider = radioRoot.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.08f, 0f);
        collider.size = new Vector3(0.6f, 0.5f, 0.45f);

        Interactable radio = AddInteractable(
            radioRoot,
            "Radio Dispatcher",
            "radio_dispatcher",
            "A radio sits under the desk lamp, its channel still open.",
            false,
            InteractableType.Dialogue);
        radio.dialogueLines = new[]
        {
            "Dispatch: You still in that room?",
            "Detective: The room is talking. It just has bad manners.",
            "Dispatch: Find the glass, the ledger, and the door. Then we can stop guessing."
        };
    }

    private PlayerClickMover CreatePlayer(Transform parent, Dictionary<string, Material> materials, Camera sceneCamera)
    {
        GameObject player = CreatePrimitive(PrimitiveType.Capsule, "Detective", parent, new Vector3(-3.35f, 0.55f, -2.2f), new Vector3(0.42f, 0.55f, 0.42f), materials["player"]);
        CreateCube("Detective Hat Brim", player.transform, new Vector3(0f, 0.72f, 0f), new Vector3(1.1f, 0.08f, 1.1f), materials["shadow"]);

        GameObject targetMarker = CreatePrimitive(PrimitiveType.Cylinder, "Move Target Marker", parent, new Vector3(-3.35f, 0.03f, -2.2f), new Vector3(0.28f, 0.01f, 0.28f), materials["accent"]);
        RemoveCollider(targetMarker);
        targetMarker.SetActive(false);

        PlayerClickMover mover = player.AddComponent<PlayerClickMover>();
        mover.sceneCamera = sceneCamera;
        mover.targetMarker = targetMarker.transform;
        mover.roomBounds = roomBounds;
        return mover;
    }

    private void BuildUi(
        Transform parent,
        out Text promptText,
        out EvidenceLog evidenceLog,
        out InspectionPanel inspectionPanel,
        out DialoguePanel dialoguePanel,
        out Text objectiveText)
    {
        Font font = GetDefaultFont();
        Color panelColor = new Color(0.035f, 0.04f, 0.045f, 0.88f);
        Color textColor = new Color(0.9f, 0.88f, 0.8f);
        Color dimTextColor = new Color(0.72f, 0.72f, 0.68f);

        GameObject canvasObject = new GameObject("UI Canvas");
        canvasObject.transform.SetParent(parent, false);
        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer >= 0)
        {
            canvasObject.layer = uiLayer;
        }

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        EnsureEventSystem(parent);

        GameObject objectivePanel = CreatePanel("Objective Panel", canvasObject.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(590f, 86f), panelColor);
        objectiveText = CreateText("Objective Text", objectivePanel.transform, font, 21, FontStyle.Bold, textColor, TextAnchor.MiddleLeft);
        Stretch(objectiveText.rectTransform, 18f, 12f, 18f, 12f);

        GameObject evidencePanel = CreatePanel("Evidence Panel", canvasObject.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -24f), new Vector2(430f, 230f), panelColor);
        Text evidenceTitle = CreateText("Evidence Title", evidencePanel.transform, font, 23, FontStyle.Bold, textColor, TextAnchor.UpperLeft);
        SetRect(evidenceTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -12f), new Vector2(-28f, 34f));
        Text evidenceEntries = CreateText("Evidence Entries", evidencePanel.transform, font, 18, FontStyle.Normal, dimTextColor, TextAnchor.UpperLeft);
        evidenceEntries.verticalOverflow = VerticalWrapMode.Overflow;
        Stretch(evidenceEntries.rectTransform, 18f, 54f, 18f, 16f);
        evidenceLog = evidencePanel.AddComponent<EvidenceLog>();
        evidenceLog.titleText = evidenceTitle;
        evidenceLog.entriesText = evidenceEntries;
        evidenceLog.ClearEvidence();

        GameObject promptPanel = CreatePanel("Prompt Panel", canvasObject.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 28f), new Vector2(720f, 46f), new Color(0.02f, 0.025f, 0.03f, 0.82f));
        promptText = CreateText("Prompt Text", promptPanel.transform, font, 20, FontStyle.Normal, textColor, TextAnchor.MiddleCenter);
        Stretch(promptText.rectTransform, 16f, 8f, 16f, 8f);
        promptText.text = "Click the floor to move. Hover a clue to inspect it.";

        inspectionPanel = CreateInspectionPanel(canvasObject.transform, font, panelColor, textColor, dimTextColor);
        dialoguePanel = CreateDialoguePanel(canvasObject.transform, font, panelColor, textColor, dimTextColor);
    }

    private InspectionPanel CreateInspectionPanel(Transform canvas, Font font, Color panelColor, Color textColor, Color dimTextColor)
    {
        GameObject panel = CreatePanel("Inspection Panel", canvas, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 260f), panelColor);
        InspectionPanel inspectionPanel = panel.AddComponent<InspectionPanel>();
        inspectionPanel.panelRoot = panel;
        inspectionPanel.titleText = CreateText("Inspection Title", panel.transform, font, 28, FontStyle.Bold, textColor, TextAnchor.UpperLeft);
        SetRect(inspectionPanel.titleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(-40f, 42f));
        inspectionPanel.bodyText = CreateText("Inspection Body", panel.transform, font, 20, FontStyle.Normal, dimTextColor, TextAnchor.UpperLeft);
        Stretch(inspectionPanel.bodyText.rectTransform, 22f, 72f, 22f, 72f);
        inspectionPanel.bodyText.verticalOverflow = VerticalWrapMode.Overflow;

        inspectionPanel.closeButton = CreateButton("Inspection Close Button", panel.transform, font, "Close", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-18f, 18f), new Vector2(120f, 42f), out _);
        inspectionPanel.closeButton.onClick.AddListener(inspectionPanel.Close);
        inspectionPanel.Close();
        return inspectionPanel;
    }

    private DialoguePanel CreateDialoguePanel(Transform canvas, Font font, Color panelColor, Color textColor, Color dimTextColor)
    {
        GameObject panel = CreatePanel("Dialogue Panel", canvas, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 92f), new Vector2(900f, 220f), panelColor);
        DialoguePanel dialoguePanel = panel.AddComponent<DialoguePanel>();
        dialoguePanel.panelRoot = panel;
        dialoguePanel.speakerText = CreateText("Speaker Text", panel.transform, font, 24, FontStyle.Bold, textColor, TextAnchor.UpperLeft);
        SetRect(dialoguePanel.speakerText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -16f), new Vector2(-40f, 34f));
        dialoguePanel.bodyText = CreateText("Dialogue Body", panel.transform, font, 21, FontStyle.Normal, dimTextColor, TextAnchor.UpperLeft);
        Stretch(dialoguePanel.bodyText.rectTransform, 24f, 62f, 24f, 70f);
        dialoguePanel.bodyText.verticalOverflow = VerticalWrapMode.Overflow;

        dialoguePanel.nextButton = CreateButton("Dialogue Next Button", panel.transform, font, "Next", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-154f, 18f), new Vector2(118f, 42f), out Text nextLabel);
        dialoguePanel.nextButtonText = nextLabel;
        dialoguePanel.closeButton = CreateButton("Dialogue Close Button", panel.transform, font, "Close", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-24f, 18f), new Vector2(118f, 42f), out _);
        dialoguePanel.nextButton.onClick.AddListener(dialoguePanel.Advance);
        dialoguePanel.closeButton.onClick.AddListener(dialoguePanel.Close);
        dialoguePanel.Close();
        return dialoguePanel;
    }

    private Interactable AddInteractable(GameObject target, string displayName, string evidenceId, string description, bool requiredForObjective, InteractableType type)
    {
        Interactable interactable = target.AddComponent<Interactable>();
        interactable.displayName = displayName;
        interactable.evidenceId = evidenceId;
        interactable.description = description;
        interactable.requiredForObjective = requiredForObjective;
        interactable.interactionType = type;
        return interactable;
    }

    private GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        return CreatePrimitive(PrimitiveType.Cube, name, parent, localPosition, localScale, material);
    }

    private GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        gameObject.name = name;
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = localPosition;
        gameObject.transform.localScale = localScale;

        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        return gameObject;
    }

    private Material CreateMaterial(string name, Color color, float metallic, float smoothness)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        if (shader == null)
        {
            shader = Shader.Find("Diffuse");
        }

        Material material = new Material(shader);
        material.name = name;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", metallic);
        }
        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", smoothness);
        }

        return material;
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;
        SetRect(panel.GetComponent<RectTransform>(), anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta);
        return panel;
    }

    private Text CreateText(string name, Transform parent, Font font, int fontSize, FontStyle fontStyle, Color color, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private Button CreateButton(string name, Transform parent, Font font, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta, out Text labelText)
    {
        GameObject buttonObject = CreatePanel(name, parent, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta, new Color(0.78f, 0.56f, 0.25f, 0.95f));
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        labelText = CreateText(label + " Label", buttonObject.transform, font, 18, FontStyle.Bold, new Color(0.08f, 0.07f, 0.055f), TextAnchor.MiddleCenter);
        Stretch(labelText.rectTransform, 4f, 4f, 4f, 4f);
        labelText.text = label;
        return button;
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
    }

    private void Stretch(RectTransform rectTransform, float left, float top, float right, float bottom)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(-right, -top);
    }

    private Font GetDefaultFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        return font;
    }

    private void EnsureEventSystem(Transform parent)
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.transform.SetParent(parent, false);
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private void RemoveCollider(GameObject target)
    {
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyGenerated(collider);
        }
    }

    private void DestroyGenerated(Object target)
    {
        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }
}
