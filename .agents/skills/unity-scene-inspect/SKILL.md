---
name: unity-scene-inspect
description: Inspect this Unity detective scene without mutating it. Use when analyzing hierarchy, finding GameObjects, checking components, reading current scene state, or preparing a safe edit. Do not use for editing objects; use unity-mcp-operating-loop and the task-specific implementation notes instead.
---

# Unity Scene Inspect

Adapted from the MIT-licensed `akiojin/unity-cli` scene inspection skill and CoderGamester MCP Unity bridge guidance.

## Use When

- The task needs current scene hierarchy or component state.
- The user asks where an object, clue, UI panel, or script is wired.
- A planned change needs a pre-edit inventory.

## Do Not Use When

- The task is pure source-code navigation.
- The task is already approved for scene mutation.
- Unity Editor is unavailable and local files answer the question.

## Preferred Flow

1. Read `GLADE.md` and `docs/game-architecture.md`.
2. Confirm the target scene is `Assets/Scenes/DetectiveRoom.unity`.
3. With MCP available, read hierarchy and console resources first.
4. Confirm generated root `__DetectiveRoomGenerated`.
5. Inspect exact GameObject/component names before suggesting edits.
6. If MCP is unavailable, inspect `DetectiveRoomBootstrap.cs` as the source of generated hierarchy truth.

## Repo-Specific Checks

- Required clues: `Broken Glass`, `Ledger`, `Locked Door`.
- Dialogue object: `Radio Dispatcher`.
- Player object: `Detective`.
- Camera object: `2.5D Orthographic Camera`.
- UI is generated under `UI Canvas`.

## References

- `docs/mcp-operating-loop.md`
- `GLADE.md`
- `Assets/Scripts/DetectiveRoomBootstrap.cs`
