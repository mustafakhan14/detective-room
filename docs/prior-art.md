# Prior Art And Borrowing Policy

Use public Unity-agent projects as reference material, but do not copy code, prompts, or docs into this repo without checking the exact license and attribution requirements first.

## Reference Targets

- `Glade-tool/glade-mcp`: primary bridge reference because it supports Unity-aware MCP tools and `GLADE.md` project context.
- `CoplayDev/unity-mcp`: reference for Unity MCP architecture, compatibility shims, paging, and test expectations.
- `CoderGamester/mcp-unity`: reference for compact MCP bridge instructions and Unity editor pitfalls.
- `akiojin/unity-cli`: reference for typed Unity CLI workflows, dry-run tool calls, and skill-oriented command docs.
- `IvanMurzak/Unity-MCP`: reference for broad Unity MCP tool coverage and project-generated skills.

## Borrowing Rules

- MIT/Apache-style repos can usually be adapted with attribution, but still inspect the repo license before copying.
- Custom licenses can impose product attribution or redistribution limits; do not lift directly until reviewed.
- Prefer reimplementing small patterns in this repo's style over copying large files.
- Keep copied snippets small, attributed, and documented in the commit message or nearby docs.
- Do not import a third-party bridge package into `Packages/manifest.json` until the repo has a clear reason to depend on it.

## Suggested Audit Workflow

Clone candidates into `/tmp/unity-agent-prior-art`, inspect `LICENSE`, `README`, and agent instruction files, then record any adopted pattern in this file before implementation.

Use:

```bash
scripts/clone-prior-art.sh
```

Current adopted patterns and license checks are recorded in `docs/prior-art-audit.md` and `THIRD_PARTY_NOTICES.md`.
