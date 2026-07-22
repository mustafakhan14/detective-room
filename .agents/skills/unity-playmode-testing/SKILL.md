---
name: unity-playmode-testing
description: Verify runtime behavior in the Unity detective prototype. Use when running EditMode or PlayMode tests, entering Play Mode, simulating the evidence loop, checking console logs, or capturing screenshot evidence. Do not use for static-only docs changes.
---

# Unity PlayMode Testing

Adapted from the MIT-licensed `akiojin/unity-cli` PlayMode testing skill and CoplayDev Unity-MCP verification flow.

## Use When

- Gameplay or UI behavior changed.
- The user asks for Play Mode confidence.
- Tests, screenshots, or console logs are required.

## Do Not Use When

- Only markdown or prompt files changed.
- Unity licensing is not warmed up and the task only needs static review.

## Preferred Flow

1. Run `scripts/verify-unity.sh`.
2. If licensing fails, open Unity Hub or this project in the editor once and rerun.
3. Run the manual Play check for behavior changes:
   - floor click moves `Detective`
   - clues inspect and add evidence once
   - radio dialogue advances and closes
   - objective completes after three required clues
4. When the MCP bridge advertises a capture tool, capture a game or scene view screenshot; otherwise use a manual Editor screenshot for visual changes.
5. Read console errors and warnings before reporting success.

## Expected Automated Coverage

- `CoreBehaviourTests` covers evidence uniqueness, objective completion, and prompt text.
- `DetectiveRoomSceneTests` opens the scene and checks generated hierarchy.
- `DetectiveRoomBootstrapPlayModeTests` builds the runtime room and checks interactables.

## References

- `scripts/verify-unity.sh`
- `docs/mcp-operating-loop.md`
- `prompts/playtest-reporter.md`
