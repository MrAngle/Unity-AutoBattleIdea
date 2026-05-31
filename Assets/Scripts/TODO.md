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
