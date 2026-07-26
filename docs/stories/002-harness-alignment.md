# Story: Align Verification And Reviewer Harness

Status: Done

## Goal

Bring the reusable verifier-evidence and layered-reviewer hardening from
`unity-agentic-starter` into the detective project without changing gameplay,
scenes, packages, project settings, or hybrid MCP routes.

## Acceptance Criteria

- [x] EditMode and PlayMode result files are invalidated before each test run,
  and malformed or internally inconsistent XML is rejected.
- [x] Deterministic fake-Unity tests cover success, stale evidence, genuine test
  failure, licensing failure, and a concurrently open Editor.
- [x] The local reviewer fails fast when its model is unavailable, caps large
  inputs, and supports both specialist and deep-review routes.
- [x] `AGENTS.md` and setup validation enforce the deep-review policy.
- [x] Detective gameplay code, assets, packages, project settings, tests, and
  hybrid MCP routing remain unchanged.

## Implementation Notes

- Keep real Unity compile, EditMode, and PlayMode results authoritative.
- Preserve `DetectiveRoom.unity`, detective-specific assertions, and the
  existing single-mutation-owner policy.
- Do not require either optional local reviewer model for deterministic checks.

## Validation Evidence

- [x] `node --test tests/agent_setup/verify-unity.test.mjs` - 8/8 passed
- [x] `tests/agent_setup/validate-agent-setup.sh`
- [x] `scripts/bridge-status.mjs --static --recommend hierarchy_inspection`
- [x] `scripts/mcp-smoke-check.sh --static`
- [x] `scripts/verify-unity.sh` - compile passed, EditMode 9/9, PlayMode 1/1
- [x] Unity-tuned reviewer skipped because no Unity C# or package changed
- [x] Live MCP passed against the exact detective project; a visual Play check
  was not required because no scene or runtime behavior changed

## Reviewer Notes

The initially open Editor targeted `DetectiveRoom.unity`, had no unsaved marker,
compiled cleanly, exposed the expected generated hierarchy, and had no Console
errors. It was closed for the batch suite and is reopened after validation.
GladeKit `0.7.16` remains pinned to the same evaluated starter baseline; the
bridge's `0.7.19` update notice is a separate package-evaluation decision.
