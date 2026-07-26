# Agent Runbook

## Before Changing Code

1. Read `AGENTS.md`, `GLADE.md`, and the relevant script files.
2. Check `git status --short` and do not overwrite user changes.
3. Identify whether the task changes gameplay, tests, docs, or tooling.
4. Prefer small edits that can be verified by Unity compile and focused tests.

## During Implementation

- Keep gameplay behavior unchanged unless explicitly requested.
- Preserve `.meta` files and avoid asset GUID churn.
- Use Unity APIs for scene, prefab, and asset changes.
- Do not add required online services or secrets.
- Use exact Unity object/script names in docs and tests.

## Verification Loop

Run:

```bash
scripts/verify-unity.sh
```

If licensing fails, open Unity Hub or the editor once, then rerun.

For gameplay changes, also manually verify:

1. Open `Assets/Scenes/DetectiveRoom.unity`.
2. Press Play.
3. Click floor movement.
4. Inspect `Broken Glass`, `Ledger`, and `Locked Door`.
5. Use `Radio Dispatcher`.
6. Confirm objective completion.

## Hybrid MCP Check

Select the route before tool use:

```bash
scripts/bridge-status.mjs --recommend hierarchy_inspection
```

After the selected bridge is configured, verify:

- Read scene hierarchy.
- Confirm `__DetectiveRoomGenerated`.
- Read console logs.
- Capture through the selected bridge when supported, or use a manual Editor screenshot for visual changes.
- For mutations, confirm one bridge owned the complete operation and any second bridge stayed read-only.
