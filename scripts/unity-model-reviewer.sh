#!/usr/bin/env bash
set -euo pipefail

MODEL="${UNITY_REVIEW_MODEL:-hf.co/parashm/Qwen2.5-Coder-7B-Instruct-Unity-Q6_K-GGUF:Q6_K}"
PROMPT_FILE="${UNITY_REVIEW_PROMPT:-prompts/unity-finetuned-reviewer.md}"

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
  input="Smoke test: identify one Unity API risk in a generated-scene detective prototype and state why compile/tests remain authoritative."
else
  input="$(cat)"
fi

if [[ -z "${input//[[:space:]]/}" ]]; then
  echo "no review input provided on stdin" >&2
  exit 1
fi

prompt="$(printf '%s\n\nReview input:\n%s\n' "$(cat "$PROMPT_FILE")" "$input")"
ollama run "$MODEL" "$prompt"
