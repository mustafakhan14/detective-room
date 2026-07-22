#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT"

fail() {
  echo "agent setup validation failed: $*" >&2
  exit 1
}

require_file() {
  [[ -f "$1" ]] || fail "missing file: $1"
}

require_executable() {
  [[ -x "$1" ]] || fail "not executable: $1"
}

require_contains() {
  local path="$1"
  local pattern="$2"
  grep -Eq "$pattern" "$path" || fail "missing pattern in $path: $pattern"
}

required_files=(
  AGENTS.md
  GLADE.md
  LICENSE
  THIRD_PARTY_NOTICES.md
  .mcp.example.json
  docs/prior-art.md
  docs/prior-art-audit.md
  docs/saad-codex-repo-setup-prompt.md
  docs/agent-workflow-80-20.md
  docs/bridge-selection.md
  docs/checklists/story-done.md
  docs/custom-mcp-extension-policy.md
  docs/mcp-operating-loop.md
  docs/mcp-smoke.md
  docs/sprint-status.yaml
  docs/stories/000-template.md
  docs/stories/001-agentic-unity-baseline.md
  docs/unity-agent-bridge.md
  docs/unity-editor-mutation-policy.md
  prompts/unity-finetuned-reviewer.md
  scripts/clone-prior-art.sh
  scripts/mcp-smoke-check.sh
  scripts/unity-model-reviewer.sh
  scripts/verify-unity.sh
)

for file in "${required_files[@]}"; do
  require_file "$file"
done

for script in scripts/clone-prior-art.sh scripts/mcp-smoke-check.sh scripts/unity-model-reviewer.sh scripts/verify-unity.sh; do
  require_executable "$script"
  bash -n "$script"
done

for generated_dir in Library Temp Obj Logs Build Builds UserSettings; do
  git check-ignore -q --no-index "${generated_dir}/.agent-setup-probe" || \
    fail "Unity generated directory is not ignored: ${generated_dir}/"
done

while IFS= read -r -d '' asset; do
  [[ -e "${asset}.meta" ]] || fail "missing Unity meta file: ${asset}.meta"
done < <(find Assets -mindepth 1 ! -name '*.meta' -print0)

node -e "for (const f of process.argv.slice(1)) JSON.parse(require('fs').readFileSync(f, 'utf8'));" \
  .mcp.example.json \
  Packages/manifest.json \
  Packages/packages-lock.json \
  Assets/Scripts/DetectiveRoom.Runtime.asmdef \
  Assets/Tests/EditMode/DetectiveRoom.EditModeTests.asmdef \
  Assets/Tests/PlayMode/DetectiveRoom.PlayModeTests.asmdef

skills=(
  unity-csharp-change
  unity-custom-mcp-extension
  unity-mcp-operating-loop
  unity-playmode-testing
  unity-scene-inspect
)

for skill in "${skills[@]}"; do
  require_file "docs/agent-skills/${skill}/SKILL.md"
  require_file ".agents/skills/${skill}/SKILL.md"
  cmp "docs/agent-skills/${skill}/SKILL.md" ".agents/skills/${skill}/SKILL.md" >/dev/null || \
    fail "skill mirror differs: ${skill}"
done

require_contains AGENTS.md "Undo\\.RecordObject"
require_contains AGENTS.md "domain reload"
require_contains AGENTS.md "port conflicts"
require_contains AGENTS.md "main thread"
require_contains AGENTS.md "tool/resource names"
require_contains Packages/manifest.json "com\\.gladekit\\.mcp-bridge"
require_contains Packages/manifest.json "57f7e1930726079e3c44475877a514758ea2545f"

for repo in "Glade-tool/glade-mcp" "CoplayDev/unity-mcp" "CoderGamester/mcp-unity" "akiojin/unity-cli" "IvanMurzak/Unity-MCP"; do
  require_contains docs/bridge-selection.md "$repo"
done

require_contains docs/bridge-selection.md "Default bridge: GladeKit MCP"
require_contains docs/unity-editor-mutation-policy.md "Undo\\.RecordObject"
require_contains docs/unity-editor-mutation-policy.md "licenseAcknowledged"
require_contains docs/custom-mcp-extension-policy.md "MCP Tool"
require_contains docs/custom-mcp-extension-policy.md "MCP Resource"
require_contains docs/custom-mcp-extension-policy.md "MCP Prompt"
require_contains docs/custom-mcp-extension-policy.md "Runtime-In-Game"
require_contains docs/mcp-smoke.md "__DetectiveRoomGenerated"
require_contains docs/mcp-smoke.md "0\.7\.16"
require_contains docs/mcp-smoke.md "manual Editor screenshot"
require_contains docs/mcp-operating-loop.md "Resource-First Loop"
require_contains docs/agent-workflow-80-20.md "Story Size"
require_contains docs/checklists/story-done.md "Unity-tuned local reviewer"
require_contains docs/sprint-status.yaml "baseline-ready"
require_contains docs/local-models.md "parashm/Qwen2\\.5-Coder-7B-Instruct-Unity-Q6_K-GGUF"
require_contains prompts/unity-finetuned-reviewer.md "Unity 6000\\.5\\.4f1"
require_contains scripts/unity-model-reviewer.sh "UNITY_REVIEW_MODEL"
require_contains scripts/verify-unity.sh "require_test_results"
require_contains scripts/verify-unity.sh "warnings"
require_contains scripts/verify-unity.sh "another Unity instance is running with this project open"
require_contains scripts/mcp-smoke-check.sh "get_scene_hierarchy"
require_contains scripts/mcp-smoke-check.sh "get_unity_console_logs"
require_contains scripts/mcp-smoke-check.sh "wrong project"
require_contains THIRD_PARTY_NOTICES.md "GladeKit MCP"
require_contains THIRD_PARTY_NOTICES.md "CoplayDev MCP for Unity"
require_contains THIRD_PARTY_NOTICES.md "CoderGamester MCP Unity"
require_contains THIRD_PARTY_NOTICES.md "akiojin unity-cli"
require_contains THIRD_PARTY_NOTICES.md "IvanMurzak Unity-MCP"
require_contains LICENSE "MIT License"
require_contains README.md "THIRD_PARTY_NOTICES\.md"
require_contains docs/prior-art-audit.md "Apache-2.0"
require_contains docs/saad-codex-repo-setup-prompt.md "I have no coding or"
require_contains docs/saad-codex-repo-setup-prompt.md "Do not stop at"
require_contains docs/saad-codex-repo-setup-prompt.md "https://github\.com/mustafakhan14/unitylearning"
require_contains docs/saad-codex-repo-setup-prompt.md "Do not manually edit Unity scene YAML"
require_contains docs/saad-codex-repo-setup-prompt.md "scripts/verify-unity\.sh"
require_contains docs/saad-codex-repo-setup-prompt.md "Unity-tuned local model as an optional reviewer"
if grep -R -q "/Users/mukhan" README.md AGENTS.md GLADE.md docs prompts scripts .mcp.example.json; then
  fail "shareable repo guidance contains a machine-specific absolute path"
fi

scripts/mcp-smoke-check.sh --static

echo "Agent setup validation passed."
