---
name: unity-mcp-operating-loop
description: Operate Unity through MCP safely. Use when using GladeKit or another Unity MCP bridge to inspect, mutate, test, or screenshot the project. Do not use when Unity Editor is unavailable and local file inspection is enough.
---

# Unity MCP Operating Loop

Adapted from MIT-licensed GladeKit MCP, CoplayDev Unity MCP, CoderGamester MCP Unity, and akiojin unity-cli workflow material.

## Use When

- The task uses a Unity MCP bridge.
- The task requires scene hierarchy, console logs, screenshots, or editor state.
- The task involves multi-step Unity editor operations.

## Do Not Use When

- A local static code read fully answers the request.
- The task only updates markdown.

## Preferred Flow

1. Confirm bridge health and active project path.
2. Read `GLADE.md` context through the bridge if available.
3. Check editor state: compiling, play mode, domain reload.
4. Read hierarchy/resources before mutating.
5. Apply the smallest tool call or batch of independent tool calls.
6. Wait for compile/domain reload after script changes.
7. Read console errors and warnings.
8. Capture screenshot evidence for visual changes when the bridge supports it; otherwise use a manual Editor screenshot.
9. Run tests or `scripts/verify-unity.sh` for behavior changes.

## GladeKit-Specific Notes

- Use `get_relevant_tools` for specialized tools instead of guessing.
- Use `search_project_scripts` when script names are unclear.
- Do not set asset import `licenseAcknowledged` without explicit user approval.
- Cloud intelligence is optional; never require `GLADEKIT_API_KEY`.

## References

- `docs/mcp-operating-loop.md`
- `docs/unity-agent-bridge.md`
- `GLADE.md`
