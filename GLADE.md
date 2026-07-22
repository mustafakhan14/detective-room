# One-Room 2.5D Detective Prototype

## Game Premise

The player is a detective investigating one compact room. The room has three essential clues and one radio dialogue interaction. The objective completes when all essential evidence is logged.

## Current Gameplay

- Click the floor to move the detective.
- Hover interactables to show prompts and highlight objects.
- Click `Broken Glass`, `Ledger`, and `Locked Door` to inspect clues and add evidence.
- Click `Radio Dispatcher` to read a short dialogue exchange.
- The objective text updates from incomplete progress to complete once all required evidence is recorded.

## Technical Context

- Unity version: `6000.5.4f1`.
- Scene: `Assets/Scenes/DetectiveRoom.unity`.
- Generated root object: `__DetectiveRoomGenerated`.
- Bootstrap script: `Assets/Scripts/DetectiveRoomBootstrap.cs`.
- UI stack: uGUI (`Canvas`, `Text`, `Button`, `CanvasScaler`).
- Physics stack: built-in 3D physics raycasts and colliders.
- Runtime assembly: `DetectiveRoom.Runtime`.

## Conventions

- Keep gameplay scripts in `Assets/Scripts/`.
- Keep tests in `Assets/Tests/EditMode/` and `Assets/Tests/PlayMode/`.
- Preserve existing plain C# MonoBehaviour style.
- Use clear object names that match gameplay language.
- Do not require cloud services or online APIs for core repo operation.
- Use `docs/agent-workflow-80-20.md` and `docs/sprint-status.yaml` for feature slices.

## Done Criteria For Agent Tasks

- Gameplay behavior remains unchanged unless the task explicitly asks for a behavior change.
- Unity compile succeeds.
- Relevant EditMode and PlayMode tests pass.
- Unity C# and package changes receive a local Unity-tuned reviewer pass when the model is available.
- The target scene opens and can be played manually.
- Console logs are checked after Unity or MCP runs.
