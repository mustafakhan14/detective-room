# Prior Art Audit

The non-AnkleBreaker reference repos were cloned to `/tmp/unity-agent-prior-art` for inspection.

## Audited Repos

| Repo | Local clone | License | Lifted into this repo |
| --- | --- | --- | --- |
| `Glade-tool/glade-mcp` | `/tmp/unity-agent-prior-art/glade-mcp` | MIT | `GLADE.md` practice, bridge docs, smoke workflow, asset-license guardrail |
| `CoplayDev/unity-mcp` | `/tmp/unity-agent-prior-art/coplaydev-unity-mcp` | MIT | resource-first MCP loop, compile/console/screenshot discipline |
| `CoderGamester/mcp-unity` | `/tmp/unity-agent-prior-art/codergamester-mcp-unity` | MIT | bridge invariants and Unity editor pitfalls |
| `akiojin/unity-cli` | `/tmp/unity-agent-prior-art/akiojin-unity-cli` | MIT | project-local skill structure and runtime checklist style |
| `IvanMurzak/Unity-MCP` | `/tmp/unity-agent-prior-art/ivanmurzak-unity-mcp` | Apache-2.0 | generated skills, custom tools/resources/prompts, runtime-in-game boundaries |

`IvanMurzak/Unity-MCP` checkout did not fully materialize because of its large filtered clone, but the git object database was usable for auditing `LICENSE`, `README.md`, `CLAUDE.md`, and file layout. Do not copy source files from it until a clean checkout completes.

## Adoption Decision

Do not vendor a whole MCP bridge into this learning project. The smarter wholesale reuse is to install GladeKit by package URL and keep this repo's own source small. The lifted material is the operating system around agentic Unity work: skills, runbooks, guardrails, verification scripts, and smoke tests.

## Copied Or Adapted Artifacts

- `docs/agent-skills/*`: project-local Unity skill playbooks adapted from akiojin's skill structure and CoplayDev's Unity-MCP operator flow.
- `docs/mcp-operating-loop.md`: resource-first MCP loop adapted from CoplayDev, GladeKit, and CoderGamester guidance.
- `docs/unity-agent-bridge.md`: GladeKit setup and smoke workflow tailored to this repo.
- `docs/custom-mcp-extension-policy.md`: custom tool/resource/prompt and runtime-in-game boundaries adapted from IvanMurzak Unity-MCP.
- `THIRD_PARTY_NOTICES.md`: license attribution for adopted patterns.
