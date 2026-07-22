# Guardrails

## Hard Rules

- Do not manually edit scene YAML, prefab YAML, or `.meta` GUIDs unless explicitly requested.
- Do not invent Unity API names. Compile or verify locally.
- Do not require cloud APIs, paid services, telemetry, or secrets for core repo operation.
- Do not modify generated Unity folders or commit ignored cache/build output.
- Do not erase or reset untracked user files.

## Unity-Specific Risks

- Unity version drift can break APIs. The pinned editor is `6000.5.4f1`.
- Test runs may fail if Unity licensing is not warmed up.
- `DetectiveRoomBootstrap` runs in edit mode; opening the scene may generate hierarchy state in memory.
- Generated object names are used by tests and agents, so renaming them is a behavior-affecting change.

## Agent Review Checklist

- Did the change preserve gameplay unless behavior changes were requested?
- Did it keep required package dependencies minimal?
- Did it avoid scene YAML edits?
- Did it preserve `.meta` files for `Assets/` content?
- Did Unity compile and relevant tests run?
- Were console logs checked?
