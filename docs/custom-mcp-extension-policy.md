# Custom MCP Extension Policy

Use this before adding project-specific MCP tools, resources, or prompts.

Adapted from the Apache-2.0 `IvanMurzak/Unity-MCP` custom tool/resource/prompt model and the MIT Unity MCP prior-art set.

## When To Extend

Add a custom MCP extension only when a workflow is repeated enough that a stable typed interface is safer than ad hoc agent instructions.

Good candidates:

- A read-only project state query used in many tasks.
- A deterministic Unity editor operation that agents often get wrong manually.
- A project-specific validation step.
- A reusable prompt that injects current project conventions or task framing.
- Runtime-in-game AI hooks for a deliberate gameplay feature.

Avoid extensions for one-off scene edits or speculative systems.

## Tool vs Resource vs Prompt

| Extension | Use for | Must not do |
| --- | --- | --- |
| MCP Tool | Mutating actions or expensive operations with typed parameters | Hide broad side effects behind vague names |
| MCP Resource | Read-only project/editor/game state | Mutate scene, assets, package state, or files |
| MCP Prompt | Reusable task framing or project-specific guidance | Replace validation, tests, or real state inspection |

## Design Rules

- Keep names explicit and stable.
- Make parameters typed, small, and described for the model.
- Validate paths and object identities before mutation.
- Return structured success/failure data.
- Keep Unity API calls on the Unity main thread.
- Run background work off the main thread when it does not touch Unity APIs.
- Add tests for every custom tool/resource/prompt.
- Document the extension in `docs/mcp-operating-loop.md` or a linked reference.

## Runtime-In-Game Rule

Runtime MCP or LLM integration is allowed only as an explicit gameplay feature. It must not become a required dependency for opening, compiling, or testing this prototype.

If added later, it needs:

- A local/offline fallback.
- A clear player-facing reason.
- No committed secrets.
- Tests for disconnected/offline behavior.

## Versioning Rule

If a project-specific extension depends on a bridge/server version, document the bridge package version, server version, and expected transport. Do not assume plugin and server versions move together.
