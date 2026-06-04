# TODO

## Tickable Inventory Item Projection

`CharacterInventory.tickableItemActions` and `tickableItemActionCache` are temporary hot-path helpers, not final architecture.

Current state:

- `InventoryRegistryIndexes.tickableItemsIndex` is the source of truth for which inventory items are tickable.
- `CharacterInventory.tickableItemActions` is a reused buffer returned to combat tick code.
- `CharacterInventory.tickableItemActionCache` caches delegates so `getTickableItems()` does not allocate a new lambda/delegate per tickable item every combat tick.

Problem:

- The cache duplicates lifecycle concerns from inventory without explicit invalidation.
- Removed tickable items can remain referenced by the cache even after they are no longer returned by `InventoryRegistryIndexes`.
- Each tick still performs dictionary lookup per tickable item.
- The design hides that the real index already lives in `InventoryRegistryIndexes`.

Preferred future direction:

- Keep `InventoryRegistryIndexes` / `InventoryRegistry` as the source of truth for tickable inventory items.
- Keep `CharacterInventory` as a thin projection/adapter between inventory and character/combat contracts.
- Avoid per-tick delegate allocation.
- Either add explicit invalidation for the current cache when inventory changes, or replace the delegate cache with a clearer character-level tickable item contract/wrapper that does not require lambda creation in the hot path.

When touching this area, do not remove the cache blindly unless the replacement still avoids per-tick allocations.

## Item Active Cells And Cell Components

Current direction:

- Item shape is becoming gameplay-relevant, not only visual placement.
- For now every occupied shape cell is treated as active.
- Flow processing capacity is derived from shape rows: each local row is one processing slot.
- Base item cast time means "cast cost per one cell"; a processing row with N cells casts for `castTime * N`.

Future active-cell direction:

- Item instances should eventually distinguish between base shape cells and active/unlocked cells.
- A low-rarity item may have only one active cell even if its shape has more cells.
- A legendary item may have all or most cells active.
- Processing rows should eventually be derived from active cells, not all shape cells.

Future component direction:

- Active cells may eventually accept cell-level overlays/components.
- Example: a golden/component cell in a wide row can reduce cast time for that processing row, amplify the effect, change damage type, or add a special rule.
- Shape remains the base potential, active cells define what is currently usable, and cell components modify the row/slot/effect.
- Keep this as future item generation/crafting work; do not add full per-cell logic until the base row-slot model is stable.

## Entry Point Trigger Presentation

Current direction:

- Normal items should use cast time as their active processing delay.
- Cooldown/recovery is intentionally not part of the active item-processing model for now.
- Entry points may still need their own trigger-time presentation because they create new flows on a rhythm.

Future UI direction:

- Show a simple countdown/charge indicator directly on entry point items for the next flow trigger.
- Keep this separate from item cast progress: trigger time creates a flow, cast time processes an existing flow through an item.

## Runtime-Created UI View Templates

Current state:

- Some UI views, especially flow/progress presentation pieces, create child objects directly in code with `new GameObject(...)`, component type lists, and string child names.
- This is acceptable for early code-driven UI prototyping, but it is brittle as a long-term pattern because styling, hierarchy, and required components are spread through view code.

Preferred future direction:

- Replace repeated manual child construction with prefab/template-backed views or a small typed UI view factory/builder.
- Prefer instantiating named view components such as progress lane, flow line, fill, comet, and track templates over relying on ad hoc string-named children.
- Keep pooling/reuse behavior for frequently updated UI elements; do not create/destroy these objects in combat/UI refresh hot paths.
- Keep this refactor presentation-only. Domain/combat runtime must not depend on prefab/template details.

## Focused Flow Path Visualization

Current direction:

- The inventory can render full active flow paths as presentation-only overlays.
- `ActiveFlowState` exposes stable domain `ActiveFlowId` and ordered processing slots, which is a good base for focused inspection later.
- Showing every active flow at once can become visually noisy when many entry points and long paths are active.
- Domain must not expose visual palette indexes, colors, opacity, dash phase, marker symbols, or highlight state.

Future UI direction:

- Add an item/entry-point focus selection mode for flow visualization.
- When an entry point is selected, show only active flow paths started by that entry point.
- When a processing item is selected, consider showing only active flows currently on or passing through that item.
- When a concrete flow is selected, show only that flow and highlight every item participating in its path.
- Use centralized UI visual states: normal, dimmed, focused, related, and stale-related.
- Prefer opacity plus mild desaturation/darkening for dimming; reserve strong outline/glow for the selected object.
- Consider recency fade for item focus: direct/current flow relations are strongest, older relations fade toward the dimmed baseline over a configured tick window.
- Keep this as a presentation/filtering concern unless gameplay rules need explicit flow ownership queries.
- Entry-point filtering likely needs stable start-entry-point data in `ActiveFlowState` and a clear UI selection model before implementation.

## Flow Path Accessibility Patterns

Current direction:

- Flow paths use a stable color from a visual-index pool.
- Shared cast rows can switch from solid lines to dashed segments so overlapping flows are distinguishable.
- Color alone should not be the final readability mechanism.

Future UI direction:

- Add non-color visual identity per flow, such as dash pattern, small repeated glyphs, or simple geometric markers.
- Prefer a small, consistent symbol set that remains readable at inventory-cell scale.
- Avoid decorative symbols that make dense flow paths harder to scan.
- This should build on stable domain `ActiveFlowId`; presentation may map that id to a temporary visual index, color, dash phase, and marker pattern for the lifetime of the visible flow.

## Flow Focus Option 3 And Details Panel

Current direction:

- Option 2 focus filtering is the near-term UX: clicking the same item, entry point, or flow toggles focus off; clicking another selectable object moves focus.
- `stale-related` is reserved in the visual state model, but tick-based recency fading is not implemented yet.
- The domain may expose stable gameplay facts such as flow id, flow kind, ordered processing path, current processing slot, owner/source data, and contribution values when they become real gameplay data.
- The domain must not expose display-only facts such as palette index, color, opacity, glow, dash style, marker glyph, or focus state.

Future UI direction:

- Option 3 should add a proper recent-context model for selected items: direct/current flow relations stay strongest, older incoming/outgoing relations fade toward the dimmed baseline over a configured tick window.
- The details panel should be driven by a presentation/read model for the currently selected item, entry point, or flow.
- Flow details should include `ActiveFlowId`, flow kind, full ordered path, current cast row, current item, owner/source entry point when available, age or created tick, and item/cast-row contribution values once tracked.
- Item details should include active/current flows, incoming and outgoing recent flow relations, current cast rows, recent received-flow ticks, and contribution breakdowns once tracked.
- Entry point details should include active flows started by it, trigger timing, recent created flows, and throughput-style summary data.
- Clicking a flow line should eventually open/select the concrete flow details while keeping the focused overlay limited to that flow.
