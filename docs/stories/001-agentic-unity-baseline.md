# Story: Agentic Unity Baseline

Status: Done

## Goal

Make the detective-room prototype safe for repeated agent work by adding durable context, guardrails, bridge setup, model-review flow, and automated Unity tests.

## Acceptance Criteria

- [x] Repo instructions exist in `AGENTS.md`.
- [x] Always-on project context exists in `GLADE.md`.
- [x] GDD, architecture, runbook, guardrails, bridge, model, and prior-art docs exist.
- [x] Unity Test Framework is configured.
- [x] EditMode and PlayMode tests cover core runtime behavior and generated scene smoke checks.
- [x] `scripts/verify-unity.sh` enforces compile and test XML success.
- [x] GladeKit MCP bridge dependency is in `Packages/manifest.json`.
- [x] Local Unity-tuned reviewer workflow is documented.

## Validation Evidence

- `scripts/verify-unity.sh`: passed with 9 EditMode tests and 1 PlayMode test.
- `tests/agent_setup/validate-agent-setup.sh`: passed.
- `scripts/mcp-smoke-check.sh --static`: passed.
- `scripts/mcp-smoke-check.sh`: passed with Unity Editor open and GladeKit listening on localhost `8765`.

## Remaining Follow-Up

- Pull the default Unity reviewer model if it is not already available locally.
