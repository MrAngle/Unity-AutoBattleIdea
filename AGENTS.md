# Project Instructions

This Unity project is `MrAngleGame`.

## Primary Context

Before making architectural or gameplay-logic decisions, read and account for:

- `Assets/AI_Context/AI_Instructions.txt`
- `Assets/AI_Context/AI_Discussions_summary.txt`
- `Assets/AI_Context/AI_DiscussionHistory.txt`
- `Assets/Scripts/TODO.md`

`Assets/AI_Context/AI_Discussions_summary.txt` is a chronological history of design decisions and architecture discussions.
`Assets/AI_Context/AI_DiscussionHistory.txt` is a running, lightweight log of current conversations, assumptions, open questions, and preliminary decisions.
`Assets/Scripts/TODO.md` tracks known architectural follow-ups that should not be treated as final architecture.
AI communication and project-context files live under `Assets/AI_Context/`, including `AI_Instructions.txt`, `AI_Csharp`, `AI_DiscussionHistory.txt`, and `AI_Discussions_summary.txt`.

Important interpretation rule:

- Treat lower/later entries in `AI_Discussions_summary.txt` as newer and more authoritative.
- If two notes conflict, prefer the later note.
- Do not treat older rejected ideas as current architecture unless a later note revives them.
- Treat `AI_DiscussionHistory.txt` as raw working context; distill durable decisions into `AI_Discussions_summary.txt` when they guide implementation.
- Treat `TODO.md` as an active list of known technical/design debt; account for relevant items before changing nearby code.
- When in doubt, summarize the apparent latest decision and ask only if the ambiguity blocks the task.
- When changing an existing mechanism, record what the previous mechanism did and how the new mechanism differs, so project history preserves the evolution of decisions.
- New entries in `AI_DiscussionHistory.txt` must start with the `AI` prefix, for example `AI YYYY-MM-DD - Topic`.
- After the user commits or pushes a change, treat the next recorded discussion as context for the next commit. Start a new simple section in `AI_DiscussionHistory.txt` so the history clearly separates what belonged to the previous commit from what is being discussed for the next one.
- Use an explicit commit separator in both AI history files whenever the user says a commit or push happened, or when recording the first discussion after such a boundary:
  `AI COMMIT BOUNDARY YYYY-MM-DD - <short context>`
- In `AI_DiscussionHistory.txt`, put raw next-commit conversation notes below that separator.
- In `AI_Discussions_summary.txt`, put only durable decisions below that separator; do not copy raw discussion unless it became architectural guidance.
- If the user provides a commit hash or branch name, include it in the separator. If not, use the date and a short human-readable label.

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
- `ICombatQueries` returning only simple values such as `int`, `float`, `bool`, `string`, enums, or similarly cheap primitives/value objects
- flow logic based on `FlowKind` and `DamageRole` semantics
- active flow lifecycle being owned by combat runtime/characters, not by entry points and not by fire-and-forget async tasks

## Combat Tick Performance

Combat tick is a hot path.

- Assume combat tick may run around 10 times per second, and potentially more often later.
- Design for a target scale of roughly 1,000-2,000 active items and flows.
- Be very careful with work done inside tick phases.
- Avoid per-tick allocations, LINQ materialization, reflection, broad collection rebuilding, and hidden O(n*m) scans in tick paths.
- Prefer stable indexes, reusable buffers, explicit tick phases, cheap primitive queries, and bounded work budgets.
- If a change may multiply cost by active flow count, active item count, or grid size, call that out before implementing.
- Do not treat `CharacterInventory.tickableItemActionCache` as a final design. It is a temporary allocation-avoidance adapter over `InventoryRegistryIndexes` and should be replaced/refined when touching tickable inventory item flow.

## Ambiguity Handling

Ask before implementing when the decision changes one of these core concepts:

- who owns active flow lifecycle
- where the global combat tick phase order is defined
- how seconds/durations map to combat ticks
- whether a rule is global combat behavior or an item-specific characteristic
- whether a query belongs on combat-wide `ICombatQueries` or a scoped character/item context

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
