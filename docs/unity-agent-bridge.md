# Unity Agent Bridge

GladeKit MCP is the default bridge for this repo because it supports Unity-aware tools and reads `GLADE.md` as durable project context.

## Install `uv`

`uvx` is required for the MCP server command. This machine uses Homebrew:

```bash
brew install uv
```

Alternative installer:

```bash
curl -LsSf https://astral.sh/uv/install.sh | sh
```

Verify:

```bash
uvx --version
```

## Add The Unity Bridge Package

The repo pins the bridge in `Packages/manifest.json`:

```json
"com.gladekit.mcp-bridge": "https://github.com/Glade-tool/glade-mcp.git?path=/unity-bridge#57f7e1930726079e3c44475877a514758ea2545f"
```

Open Unity once after this dependency is added so Package Manager resolves `Packages/packages-lock.json`.

Manual fallback in Unity:

1. Open `Window > Package Manager`.
2. Click `+`.
3. Choose `Add package from git URL...`.
4. Paste:

```text
https://github.com/Glade-tool/glade-mcp.git?path=/unity-bridge
```

The bridge should listen on `localhost:8765` when the editor is open.

## Configure MCP Client

Use this MCP server command:

```json
{
  "mcpServers": {
    "gladekit-mcp": {
      "command": "uvx",
      "args": ["gladekit-mcp"]
    }
  }
}
```

Do not commit API keys or personal MCP client config. If semantic script search is later enabled with an OpenAI key, keep the key in local client environment settings only.

## Smoke Test

Ask the agent to:

- Read the active scene hierarchy.
- Confirm `__DetectiveRoomGenerated`.
- Read Unity console logs.
- Capture a scene or game view screenshot when the advertised bridge tools support it; otherwise use a manual Editor screenshot for visual changes.

The pinned bridge `0.7.16` does not currently advertise a screenshot tool from
its Unity `/api/tools/list` endpoint. Hierarchy and console access remain the
required automated smoke checks.

## Operating Loop

Use `docs/mcp-operating-loop.md` for the resource-first workflow adapted from the audited Unity MCP projects.

When using GladeKit asset import tools, do not set `licenseAcknowledged: true` until the user explicitly approves the exact asset and license candidate.
