# Game Architecture

## Runtime Shape

The scene is intentionally generated at runtime/editor time by `DetectiveRoomBootstrap`. This keeps the Unity scene small and makes the prototype easy for agents and humans to inspect.

## Main Systems

- `DetectiveRoomBootstrap` creates the camera, lights, room shell, props, interactables, player, UI, and objective wiring.
- `PlayerClickMover` handles floor-click navigation using camera rays and a floor plane.
- `InteractionController` raycasts from the cursor, highlights hovered interactables, and routes clicks.
- `Interactable` stores clue/dialogue metadata and applies highlight property blocks.
- `EvidenceLog` stores unique evidence IDs and refreshes the evidence UI.
- `GameObjective` tracks required evidence completion.
- `InspectionPanel` and `DialoguePanel` own modal uGUI panels.

## Data Flow

1. `DetectiveRoomBootstrap` creates interactables and required evidence IDs.
2. `InteractionController` calls `Interact()` after a click on an interactable.
3. Clues show an inspection panel and call `EvidenceLog.AddEvidence()`.
4. `EvidenceLog.EvidenceAdded` notifies `GameObjective`.
5. `GameObjective` updates objective text and completion state.

## Unity Constraints

- Keep scene mutation inside Unity APIs, not manual YAML edits.
- Keep generated object names stable; tests and agents use them.
- Keep uGUI until the project explicitly migrates.
- Keep package dependencies minimal.

## Test Strategy

- EditMode tests cover small pure-behavior seams: evidence uniqueness, objective progress, and prompt text.
- EditMode scene smoke test opens `DetectiveRoom.unity` and confirms generated hierarchy is present.
- PlayMode smoke test creates a bootstrap host and validates generated runtime objects and interactables.
