#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_VERSION="${UNITY_VERSION:-6000.5.4f1}"
UNITY_BIN="${UNITY_BIN:-/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity}"
LOG_DIR="${PROJECT_ROOT}/Logs/Verification"
RESULTS_DIR="${PROJECT_ROOT}/Logs/TestResults"
UNITY_TIMEOUT_SECONDS="${UNITY_TIMEOUT_SECONDS:-600}"

mkdir -p "$LOG_DIR" "$RESULTS_DIR"

if [[ ! -x "$UNITY_BIN" ]]; then
  echo "Unity editor not found or not executable: $UNITY_BIN" >&2
  echo "Install Unity ${UNITY_VERSION} or set UNITY_BIN to the editor executable." >&2
  exit 127
fi

run_unity() {
  local name="$1"
  shift
  local log_file="${LOG_DIR}/${name}.log"
  local output_file="${LOG_DIR}/${name}.output.log"

  echo "==> ${name}"
  : > "$log_file"
  : > "$output_file"
  "$UNITY_BIN" \
    -batchmode \
    -projectPath "$PROJECT_ROOT" \
    -logFile "$log_file" \
    "$@" > "$output_file" 2>&1 &
  local unity_pid=$!
  local deadline=$((SECONDS + UNITY_TIMEOUT_SECONDS))

  while kill -0 "$unity_pid" 2>/dev/null; do
    if grep -Eqs "Licensing initialization failed|Failed to connect to LicenseClient" "$log_file" "$output_file"; then
      kill "$unity_pid" 2>/dev/null || true
      wait "$unity_pid" 2>/dev/null || true
      echo "Unity licensing is not ready for batchmode." >&2
      echo "Open Unity Hub or this project in the Unity Editor once, then rerun scripts/verify-unity.sh." >&2
      echo "Log: $log_file" >&2
      echo "Output: $output_file" >&2
      exit 70
    fi

    if (( SECONDS >= deadline )); then
      kill "$unity_pid" 2>/dev/null || true
      wait "$unity_pid" 2>/dev/null || true
      echo "Unity command timed out after ${UNITY_TIMEOUT_SECONDS}s: ${name}" >&2
      echo "Log: $log_file" >&2
      echo "Output: $output_file" >&2
      exit 124
    fi

    sleep 2
  done

  set +e
  wait "$unity_pid"
  local status=$?
  set -e

  if grep -Eqs "Licensing initialization failed|Failed to connect to LicenseClient" "$log_file" "$output_file"; then
    echo "Unity licensing is not ready for batchmode." >&2
    echo "Open Unity Hub or this project in the Unity Editor once, then rerun scripts/verify-unity.sh." >&2
    echo "Log: $log_file" >&2
    echo "Output: $output_file" >&2
    exit 70
  fi

  if grep -Eqs "Scripts have compiler errors|Compilation failed|error CS[0-9]{4}" "$log_file" "$output_file"; then
    echo "Unity compile errors detected. Tail of log:" >&2
    tail -n 80 "$log_file" >&2
    echo "Output: $output_file" >&2
    exit 1
  fi

  if grep -Eqs "another Unity instance is running with this project open|Multiple Unity instances cannot open the same project" "$log_file" "$output_file"; then
    echo "Unity Editor already has this project open." >&2
    echo "Close ${PROJECT_ROOT} in the Unity Editor, then rerun scripts/verify-unity.sh." >&2
    echo "Log: $log_file" >&2
    echo "Output: $output_file" >&2
    exit 71
  fi

  if [[ $status -ne 0 ]]; then
    echo "Unity command failed with exit code ${status}. Tail of log:" >&2
    if [[ -s "$output_file" ]]; then
      echo "Tail of output:" >&2
      tail -n 80 "$output_file" >&2
    fi
    if [[ -f "$log_file" ]]; then
      tail -n 80 "$log_file" >&2
    fi
    exit "$status"
  fi

  echo "    log: $log_file"
  echo "    output: $output_file"
}

require_test_results() {
  local name="$1"
  local result_file="$2"

  if [[ ! -s "$result_file" ]]; then
    echo "Unity ${name} test results were not written: $result_file" >&2
    exit 72
  fi

  node -e '
    const fs = require("fs");
    const [name, file] = process.argv.slice(1);
    const xml = fs.readFileSync(file, "utf8");
    const run = xml.match(/<test-run\b[^>]*>/);
    if (!run) {
      console.error(`Unity ${name} results are missing <test-run>: ${file}`);
      process.exit(72);
    }

    const attrs = Object.fromEntries([...run[0].matchAll(/\s([A-Za-z-]+)="([^"]*)"/g)].map((m) => [m[1], m[2]]));
    const total = Number(attrs.total || 0);
    const failed = Number(attrs.failed || 0);
    const warnings = Number(attrs.warnings || 0);
    const inconclusive = Number(attrs.inconclusive || 0);
    const skipped = Number(attrs.skipped || 0);
    const result = attrs.result || "Unknown";

    console.log(`${name}: result=${result} total=${total} passed=${attrs.passed || 0} failed=${failed} warnings=${warnings} inconclusive=${inconclusive} skipped=${skipped}`);

    if (result !== "Passed" || total <= 0 || failed !== 0 || warnings !== 0 || inconclusive !== 0 || skipped !== 0) {
      console.error(`Unity ${name} tests did not fully pass: ${file}`);
      process.exit(1);
    }
  ' "$name" "$result_file"
}

run_unity compile -quit
run_unity editmode-tests -runTests -testPlatform EditMode -testResults "${RESULTS_DIR}/editmode-results.xml"
require_test_results EditMode "${RESULTS_DIR}/editmode-results.xml"
run_unity playmode-tests -runTests -testPlatform PlayMode -testResults "${RESULTS_DIR}/playmode-results.xml"
require_test_results PlayMode "${RESULTS_DIR}/playmode-results.xml"

echo "Unity verification passed."
