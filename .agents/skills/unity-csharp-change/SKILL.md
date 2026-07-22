---
name: unity-csharp-change
description: Change Unity C# scripts safely in this repo. Use when editing MonoBehaviours, tests, asmdefs, or compile-affecting code. Do not use for scene-only inspection or docs-only tasks.
---

# Unity C# Change

Adapted from the MIT-licensed CoplayDev Unity-MCP operator guide and CoderGamester MCP Unity invariants.

## Use When

- A task edits `Assets/Scripts/`.
- A task edits Unity tests or asmdefs.
- A task changes serialized fields, public methods, or component wiring.

## Do Not Use When

- The task is only writing docs or prompts.
- The task only needs current scene hierarchy.

## Preferred Flow

1. Read the target script and dependent scripts.
2. Keep changes small and compile-oriented.
3. Do not attach or depend on new scripts until Unity compilation succeeds.
4. Run JSON/shell checks if asmdefs or scripts changed.
5. Run `scripts/verify-unity.sh`.
6. Check console output after compilation.

## Local Invariants

- Runtime assembly is `DetectiveRoom.Runtime`.
- uGUI requires the `UnityEngine.UI` asmdef reference.
- Test asmdefs use `UNITY_INCLUDE_TESTS` and NUnit.
- Keep MonoBehaviours in the global namespace for now.
- Do not add package dependencies without updating `Packages/packages-lock.json`.

## References

- `AGENTS.md`
- `docs/game-architecture.md`
- `Assets/Scripts/DetectiveRoom.Runtime.asmdef`
