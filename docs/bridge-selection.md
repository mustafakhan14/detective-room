# Unity Agent Bridge Selection

Use this matrix before adding or changing a Unity editor bridge.

## Default

Use GladeKit MCP first for this repo.

Why:

- It reads `GLADE.md` from the Unity project root.
- It exposes broad Unity-aware tools, resources, console access, and project script search; screenshot availability depends on the installed bridge/client version.
- It keeps the bridge as a package dependency instead of vendoring a bridge implementation into this learning repo.
- It can run fully local for core features.

Install path:

```text
https://github.com/Glade-tool/glade-mcp.git?path=/unity-bridge
```

MCP server command:

```bash
uvx gladekit-mcp
```

## Fallbacks

| Option | Use when | Avoid when |
| --- | --- | --- |
| `Glade-tool/glade-mcp` | Default Unity MCP bridge, `GLADE.md` context, script search, broad tools | You need built-in screenshot capture, want to avoid installing `uv`, or the package URL fails |
| `CoplayDev/unity-mcp` | You need mature Unity MCP operator docs, resource-first workflows, broad test/harness ideas | You do not want a Python/FastMCP bridge |
| `CoderGamester/mcp-unity` | You want a compact Node/WebSocket bridge model and simple tool/resource contracts | You need the broader GladeKit tool surface |
| `akiojin/unity-cli` | MCP is unavailable but a typed CLI workflow, dry-run calls, or command schemas would help | You need MCP-native integration |
| `IvanMurzak/Unity-MCP` | Generated skills, custom tools/resources/prompts, runtime-in-game patterns, and server/plugin separation | Do not copy source files until a clean checkout completes |

## Decision Rules

- Do not vendor a full bridge implementation into this repo without a specific feature gap that package installation cannot solve.
- Prefer package/CLI installation plus repo-specific docs, prompts, tests, and skills.
- Keep all bridge credentials and personal client config out of git.
- If adding a bridge package, update `Packages/manifest.json`, `Packages/packages-lock.json`, `docs/unity-agent-bridge.md`, and this file.
- If a bridge changes ports or tool names, update `docs/mcp-operating-loop.md` and `scripts/mcp-smoke-check.sh`.
- If adding custom project MCP tools/resources/prompts, follow `docs/custom-mcp-extension-policy.md`.

## Current Status

- Default bridge: GladeKit MCP.
- Example MCP config: `.mcp.example.json`.
- Local smoke readiness script: `scripts/mcp-smoke-check.sh`.
- Full Unity verification script: `scripts/verify-unity.sh`.
