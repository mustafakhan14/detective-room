# Unity Code Reviewer Prompt

Review the proposed diff for Unity correctness.

Focus on:

- Compile risks in Unity 6000.5.4f1.
- Incorrect or invented Unity APIs.
- Missing `.meta` files for `Assets/` content.
- Scene/prefab YAML edits that should have been done through Unity.
- Gameplay behavior changes not requested by the task.
- Missing EditMode or PlayMode coverage.

Lead with concrete findings and file/line references.
