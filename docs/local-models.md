# Local Models

Use local models as layered reviewers for Unity API risk. They are not implementation authorities; Unity compile, tests, MCP/editor state, and local package code remain the source of truth.

## Default 80/20 Specialist

Use `parashm/Qwen2.5-Coder-7B-Instruct-Unity-Q6_K-GGUF:Q6_K` through Ollama or another GGUF runtime.

Recommended pull command:

```bash
ollama pull hf.co/parashm/Qwen2.5-Coder-7B-Instruct-Unity-Q6_K-GGUF:Q6_K
```

Recommended specialist smoke command:

```bash
scripts/unity-model-reviewer.sh --smoke
```

Recommended review command after Unity C# or package changes:

```bash
git diff -- Assets/Scripts Assets/Tests Packages ProjectSettings | scripts/unity-model-reviewer.sh
```

The 7B Unity-tuned model is the fast first pass. It is useful for API skepticism, but its verdict can understate cross-cutting runtime/editor or architectural failures.

## Deep Reviewer

Use the broader `qwen3.6:latest` model for high-risk changes, ambiguous specialist findings, runtime/editor assembly boundaries, package changes, or before a release checkpoint:

```bash
git diff -- Assets/Scripts Assets/Tests Packages ProjectSettings | scripts/unity-model-reviewer.sh --deep
```

Verify that the optional deep model is installed with:

```bash
scripts/unity-model-reviewer.sh --deep --check
```

The deep reviewer is slower and not Unity-fine-tuned. It complements the specialist; it does not replace deterministic Unity validation.

If the default model is still downloading, the script can be smoke-tested with another installed Ollama model:

```bash
UNITY_REVIEW_MODEL=qwen2.5-coder:14b scripts/unity-model-reviewer.sh --smoke
```

That fallback validates the workflow path only. It is not a substitute for the layered review policy.

## Role In This Repo

Ask the model to find:

- invented Unity APIs
- editor-only APIs used at runtime
- missing package dependencies
- Unity 6000.5.4f1 compatibility risks
- missing `.meta` files or asset GUID churn
- tests that do not exercise the changed behavior

Do not ask it to:

- rewrite the whole feature
- make final merge decisions
- replace `scripts/verify-unity.sh`
- mutate scenes, packages, or assets

## Larger Candidate

`wrayy/Qwenity3.6-27B-msv2` is a later evaluation candidate when local hardware and serving are proven. It should not block normal work because it is materially heavier and less convenient than the Q6_K GGUF reviewer path.

## Repo-Specific Fine-Tuning Policy

Do not start a repo-specific fine-tune until this project has:

- 20 to 50 representative failed and successful Unity-agent tasks
- expected findings for each task
- Unity compile/test outcomes for each task
- MCP screenshot or console evidence for scene tasks
- a repeatable evaluator script that compares reviewer output against expected findings

Until then, the high-leverage path is a Unity-tuned reviewer plus deterministic verification.

## Current 80/20 Model Policy

1. Codex remains the main planner and implementation agent.
2. Run the Unity-tuned 7B reviewer after Unity C# or package changes.
3. Add `--deep` for risky, cross-cutting, or disputed changes.
4. Treat both model outputs as advisory and resolve them with compile, EditMode, PlayMode, console, MCP state, and screenshots where relevant.
5. Do not download a larger Unity model until a repo-specific evaluation set shows a likely gain.

## Source Notes

- `neph1/Qwen2.5-Coder-7B-Instruct-Unity` is Apache-2.0 and based on Unity-related datasets.
- `parashm/Qwen2.5-Coder-7B-Instruct-Unity-Q6_K-GGUF` is an Apache-2.0 GGUF packaging path with Ollama/llama.cpp examples.
- `vishnuOI/unity-coder-7b` is a Qwen2.5-Coder-7B Unity C# fine-tune on `vishnuOI/unity-dev-instructions`, but its model card lists CC-BY-4.0, so treat attribution carefully.
- `vishnuOI/unity-dev-instructions` and `Hypersniper/unity_api_2022_3` are useful evaluation-set references, not datasets to copy wholesale into this repo.
