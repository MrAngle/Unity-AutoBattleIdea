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
