---
name: unity-custom-mcp-extension
description: Design project-specific Unity MCP tools, resources, or prompts. Use when repeated repo workflows need typed bridge capabilities, generated skills, runtime-in-game hooks, or reusable MCP prompts. Do not use for one-off scene edits or ordinary MonoBehaviour changes.
---

# Unity Custom MCP Extension

Adapted from Apache-2.0 `IvanMurzak/Unity-MCP` custom tool/resource/prompt guidance and the audited Unity MCP operating-loop docs.

## Use When

- A workflow should become a reusable MCP Tool, Resource, or Prompt.
- Agents repeatedly need the same project-specific Unity state query.
- A runtime-in-game AI hook is being deliberately designed.
- Generated skills or tool docs need to describe a stable project capability.

## Do Not Use When

- The task is a one-off gameplay script edit.
- A normal Unity test or local script is enough.
- The feature would require cloud services for core repo operation.

## Preferred Flow

1. Decide whether the capability is a Tool, Resource, or Prompt.
2. Define the minimum typed interface and structured return shape.
3. State which parts must run on Unity's main thread.
4. Add tests before relying on the extension in agent workflows.
5. Update `docs/custom-mcp-extension-policy.md` and relevant runbooks.
6. Add or update a skill only after the interface is stable.

## Repo Defaults

- Prefer resources for read-only scene/project state.
- Prefer tools only for deterministic, validated mutations.
- Prefer prompts for repeatable task framing, not hidden behavior.
- Runtime-in-game integration is optional and must have an offline fallback.

## References

- `docs/custom-mcp-extension-policy.md`
- `docs/mcp-operating-loop.md`
- `docs/bridge-selection.md`
