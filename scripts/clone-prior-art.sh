#!/usr/bin/env bash
set -euo pipefail

DEST="${1:-/tmp/unity-agent-prior-art}"
mkdir -p "$DEST"

clone_or_update() {
  local repo="$1"
  local dir="$2"

  if [[ -d "$dir/.git" ]]; then
    echo "==> Updating $repo"
    git -C "$dir" pull --ff-only
    return
  fi

  echo "==> Cloning $repo"
  git clone --depth 1 "$repo" "$dir"
}

clone_or_update https://github.com/Glade-tool/glade-mcp.git "$DEST/glade-mcp"
clone_or_update https://github.com/CoplayDev/unity-mcp.git "$DEST/coplaydev-unity-mcp"
clone_or_update https://github.com/CoderGamester/mcp-unity.git "$DEST/codergamester-mcp-unity"
clone_or_update https://github.com/akiojin/unity-cli.git "$DEST/akiojin-unity-cli"
clone_or_update https://github.com/IvanMurzak/Unity-MCP.git "$DEST/ivanmurzak-unity-mcp"

echo "Prior-art repos are available in $DEST"
