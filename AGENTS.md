# Project Instructions

This Unity project is `MrAngleGame`.

## Primary Context

Before making architectural or gameplay-logic decisions, read and account for:

- `Assets/AI_Instructions.txt`
- `Assets/Scripts/AI_Discussions_summary.txt`

`Assets/Scripts/AI_Discussions_summary.txt` is a chronological history of design decisions and architecture discussions.

Important interpretation rule:

- Treat lower/later entries in `AI_Discussions_summary.txt` as newer and more authoritative.
- If two notes conflict, prefer the later note.
- Do not treat older rejected ideas as current architecture unless a later note revives them.
- When in doubt, summarize the apparent latest decision and ask only if the ambiguity blocks the task.

## Architecture Direction

Prefer the existing modular-monolith style:

- domain logic split by feature modules such as `Flow`, `CombatContext`, `Character`, `Inventory`, `Item`, and `ActionExecutor`
- public contracts and APIs at module boundaries
- internal domain implementations where practical
- dependency composition through Zenject installers
- UI separated from domain/gameplay logic

Keep changes aligned with the current direction from `AI_Discussions_summary.txt`, especially around:

- deterministic combat/runtime state
- `CombatCommand` as an intent/request
- `CombatEvent` as a domain fact
- `CombatHook` as a reaction to facts
- `ICombatCapabilities` split into command/query sides
- flow logic based on `FlowKind` and `DamageRole` semantics

## Coding Preferences

- Follow the style already present in the touched files.
- Keep Unity-specific code out of pure domain code when reasonably possible.
- Prefer small, explicit domain APIs over overly generic abstractions.
- Avoid adding new architectural layers unless they remove real complexity.
- When introducing tests, prefer deterministic EditMode tests for domain logic.

## Repository Hygiene

- Do not rewrite unrelated files.
- Preserve Unity `.meta` files.
- Be careful with assembly definition references.
- If generated Unity project files change incidentally, do not treat them as architectural source of truth.
