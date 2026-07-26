#!/usr/bin/env bash
set -euo pipefail

DEFAULT_MODEL="${UNITY_SPECIALIST_REVIEW_MODEL:-hf.co/parashm/Qwen2.5-Coder-7B-Instruct-Unity-Q6_K-GGUF:Q6_K}"
DEEP_MODEL="${UNITY_DEEP_REVIEW_MODEL:-qwen3.6:latest}"
MODEL="${UNITY_REVIEW_MODEL:-$DEFAULT_MODEL}"
PROMPT_FILE="${UNITY_REVIEW_PROMPT:-prompts/unity-finetuned-reviewer.md}"

if [[ "${1:-}" == "--deep" ]]; then
  MODEL="$DEEP_MODEL"
  shift
fi

if ! command -v ollama >/dev/null 2>&1; then
  echo "ollama is not on PATH. Install Ollama or set up another GGUF runtime." >&2
  exit 127
fi

if [[ "${1:-}" == "--pull" ]]; then
  ollama pull "$MODEL"
  exit 0
fi

if [[ "${1:-}" == "--check" ]]; then
  ollama list | grep -F "$MODEL" >/dev/null || {
    echo "model is not available locally: $MODEL" >&2
    echo "Run: scripts/unity-model-reviewer.sh --pull" >&2
    exit 2
  }
  echo "Unity reviewer model is available: $MODEL"
  exit 0
fi

if [[ ! -f "$PROMPT_FILE" ]]; then
  echo "missing reviewer prompt: $PROMPT_FILE" >&2
  exit 1
fi

if [[ "${1:-}" == "--smoke" ]]; then
  input="$(cat <<'EOF'
Synthetic review smoke test. This is test input, not repository code:

diff --git a/Assets/Scripts/BadUnityChange.cs b/Assets/Scripts/BadUnityChange.cs
new file mode 100644
--- /dev/null
+++ b/Assets/Scripts/BadUnityChange.cs
@@
+using UnityEditor;
+using UnityEngine;
+
+public class BadUnityChange : MonoBehaviour
+{
+    private void Update()
+    {
+        AssetDatabase.Refresh();
+        Physics.RaycastAsync(transform.position, Vector3.forward);
+    }
+}

Identify the editor/runtime boundary violation and invented API, then return a block verdict. State that Unity compile/tests remain authoritative.
EOF
)"
else
  input="$(cat)"
fi

if [[ -z "${input//[[:space:]]/}" ]]; then
  echo "no review input provided on stdin" >&2
  exit 1
fi

prompt="$(printf '%s\n\nReview input:\n%s\n' "$(cat "$PROMPT_FILE")" "$input")"
ollama run "$MODEL" "$prompt" --hidethinking
