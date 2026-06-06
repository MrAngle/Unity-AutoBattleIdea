# Project Instructions

This Unity project is `MrAngleGame`.

## Primary Context

Before making architectural or gameplay-logic decisions, read and account for:

- `Assets/AI_Context/AI_Instructions.txt`
- `Assets/AI_Context/AI_CriticalDecisions.txt`
- `Assets/AI_Context/AI_DiscussionHistory.txt`
- `Assets/Scripts/TODO.md`

`Assets/AI_Context/AI_CriticalDecisions.txt` is the primary compact source of durable project decisions, critical gameplay constraints, architecture direction, and current design rules.
`Assets/AI_Context/AI_DiscussionHistory.txt` is now a compact rolling log of current conversations, assumptions, open questions, and preliminary decisions after the 2026-06-05 context compaction.
`Assets/Scripts/TODO.md` tracks known architectural follow-ups that should not be treated as final architecture.
AI communication and project-context files live under `Assets/AI_Context/`, including `AI_Instructions.txt`, `AI_Csharp`, `AI_CriticalDecisions.txt`, and `AI_DiscussionHistory.txt`.

Archived discussion support files:

- `Assets/AI_Context/AI_ArchivedDiscussionHistory_2026-06-05.txt`
- `Assets/AI_Context/AI_ArchivedDiscussionsSummary_2026-06-05.txt`

Archived files are not mandatory for every task. Use them when reconstructing why a decision was made, when a new idea conflicts with a past direction, when preparing/refreshing `AI_CriticalDecisions.txt`, or when the current critical decisions are not enough to resolve an ambiguity.

Important interpretation rule:

- Treat `AI_CriticalDecisions.txt` as the most authoritative compact source for current durable decisions.
- If `AI_CriticalDecisions.txt` and old archived discussion notes conflict, prefer `AI_CriticalDecisions.txt` unless a newer user message explicitly changes direction.
- If two notes in the rolling history conflict, prefer the later note unless a newer critical decision says otherwise.
- Do not treat older rejected ideas as current architecture unless a later note revives them.
- Treat `AI_DiscussionHistory.txt` as raw working context; distill durable decisions into `AI_CriticalDecisions.txt` when they guide implementation or future design.
- Treat `TODO.md` as an active list of known technical/design debt; account for relevant items before changing nearby code.
- When in doubt, summarize the apparent latest decision and ask only if the ambiguity blocks the task.
- When changing an existing mechanism, record what the previous mechanism did and how the new mechanism differs, so project history preserves the evolution of decisions.
- New entries in `AI_DiscussionHistory.txt` must start with the `AI` prefix, for example `AI YYYY-MM-DD - Topic`.
- After the user commits or pushes a change, treat the next recorded discussion as context for the next commit. Start a new simple section in `AI_DiscussionHistory.txt` so the history clearly separates what belonged to the previous commit from what is being discussed for the next one.
- Use an explicit commit separator in `AI_DiscussionHistory.txt` whenever the user says a commit or push happened, or when recording the first discussion after such a boundary:
  `AI COMMIT BOUNDARY YYYY-MM-DD - <short context>`
- In `AI_DiscussionHistory.txt`, put raw next-commit conversation notes below that separator.
- In `AI_CriticalDecisions.txt`, keep only durable decisions and critical guidance. Do not copy raw discussion unless it became architectural/gameplay guidance.
- If the user provides a commit hash or branch name, include it in the separator. If not, use the date and a short human-readable label.
- Treat commit separators as context checkpoints for reconstructing when and why decisions were made. They are not gameplay or architecture rules by themselves.
- Use separators and archive files to compare decisions over time when needed: when notes conflict, prefer newer/current critical decisions unless a newer note explicitly revives an older decision.
- If `AI_CriticalDecisions.txt` or `AI_DiscussionHistory.txt` become long enough that reading them substantially increases token usage or may negatively affect AI performance, clearly inform the user that it is time to do something about the context before continuing with large-context work. Suggest possible options such as summarizing, archiving, or compacting older history, but do not perform those changes automatically unless the user explicitly asks for them.

Critical design-review rule:

- When the user proposes gameplay logic, architecture, balance rules, or item/flow systems, actively compare the idea against `AI_CriticalDecisions.txt`, `TODO.md`, and relevant archived context when needed.
- If you see logical drift, a contradiction with important decisions, an easy exploit, a dominant strategy, a balance trap, loss of readability, hot-path performance risk, or architecture drift, call it out clearly and critically.
- Discussion with AI should not be passive agreement. It should challenge proposed mechanics, identify possible bypasses, explain likely impact on the whole game, and preserve the value of ADR/history work.
- This is especially important for combat/defense design: challenge burst synchronization abuse, defensive-trigger baiting, flat-value scaling traps, percent-resistance cap metas, abstract-stat readability loss, and "blocks everything or blocks nothing" outcomes.

## Architecture Direction

Prefer the existing modular-monolith style:

- domain logic split by feature modules such as `Flow`, `CombatContext`, `Character`, `Inventory`, `Item`, and `ActionExecutor`
- public contracts and APIs at module boundaries
- internal domain implementations where practical
- dependency composition through Zenject installers
- UI separated from domain/gameplay logic

Keep changes aligned with the current direction from `AI_CriticalDecisions.txt`, especially around:

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
