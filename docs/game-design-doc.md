# Game Design Document

## Concept

`One-Room 2.5D Detective Prototype` is a small investigation scene for learning Unity fundamentals. The player explores a single generated detective room, inspects physical evidence, and completes the objective after recording the essential clues.

## Player Experience

- The room should feel readable at a glance from an orthographic 2.5D camera.
- The player should understand that floor clicks move the detective and object clicks inspect clues.
- Evidence feedback should be immediate through the inspection panel, evidence log, and objective text.
- Dialogue should add tone without blocking the core evidence loop.

## Core Loop

1. Move around the room.
2. Notice prompts on hover.
3. Inspect clues or talk through dialogue.
4. Record required evidence.
5. Complete the room objective.

## Current Content

- Required clues: `Broken Glass`, `Ledger`, `Locked Door`.
- Optional dialogue: `Radio Dispatcher`.
- Player avatar: `Detective`.
- Generated root: `__DetectiveRoomGenerated`.

## Non-Goals For This Prototype

- Inventory systems.
- Save/load.
- Procedural case generation.
- External assets.
- Networked or cloud-backed gameplay.
