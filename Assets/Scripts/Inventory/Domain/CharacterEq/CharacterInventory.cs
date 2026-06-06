using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain.CharacterEq {
    internal class CharacterInventory : ICharacterInventory {
        private InventoryAggregate inventoryAggregate;

        private readonly Dictionary<IInventoryCombatTickableItem, CharacterCombatTickableItemAction>
            tickableItemActionCache = new();

        private readonly Dictionary<Id<ItemId>, ICharacterEquippedItem> equippedItemByItemId = new();

        private readonly List<CharacterCombatTickableItemAction> tickableItemActions = new();

        public CharacterInventory(InventoryAggregate inventoryAggregate) {
            this.inventoryAggregate = NullGuard.NotNullOrThrow(inventoryAggregate);
        }

        public IEnumerable<IGridItemPlaced> getPlacedSnapshot() {
            return inventoryAggregate.getPlacedSnapshot();
        }

        public IReadOnlyInventoryGrid getInventoryGrid() {
            return inventoryAggregate.getInventoryGrid();
        }

        public bool tryGetItemAtCell(Vector2Int cell, out ICharacterEquippedItem item) {
            if (inventoryAggregate.tryGetItemAtCell(cell, out IInventoryPlacedItem inventoryPlacedItem)) {
                item = getOrCreateCharacterEquippedItem(inventoryPlacedItem);
                return true;
            }

            item = null;
            return false;
        }

        public bool tryGetItemById(Id<ItemId> itemId, out ICharacterEquippedItem item) {
            NullGuard.ValidIdOrThrow(itemId);

            if (!inventoryAggregate.tryGetItemById(itemId, out IInventoryPlacedItem inventoryPlacedItem)) {
                item = null;
                return false;
            }

            item = getOrCreateCharacterEquippedItem(inventoryPlacedItem);
            return true;
        }

        public bool tryGetEntryPointById(Id<ItemId> itemId, out ICharacterEquippedEntryPoint entryPoint) {
            NullGuard.ValidIdOrThrow(itemId);

            if (inventoryAggregate.tryGetEntryPointById(itemId,
                    out IInventoryPlacedEntryPoint inventoryPlacedEntryPoint)) {
                ICharacterEquippedItem equippedItem = getOrCreateCharacterEquippedItem(inventoryPlacedEntryPoint);
                entryPoint = equippedItem as ICharacterEquippedEntryPoint;

                if (entryPoint == null) {
                    throw new InvalidOperationException(
                        $"Item '{itemId}' is registered as entry point but cached as regular equipped item.");
                }

                return true;
            }

            entryPoint = null;
            return false;
        }

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand) {
            return getOrCreateCharacterEquippedItem(inventoryAggregate.place(placeItemCommand));
        }

        public bool canPlace(PlaceItemQuery placeItemCommand) {
            return inventoryAggregate.canPlace(placeItemCommand);
        }

        public IReadOnlyCollection<ICharacterEquippedEntryPoint> getEntryPoints() {
            IReadOnlyCollection<IInventoryPlacedEntryPoint> aggregateSet = inventoryAggregate.getEntryPoints();
            return mapToEquippedEntryPoints(aggregateSet);
        }

        public bool tryGetNeighborItems(IGridItemPlaced sourceGridItemPlaced,
                                        IEnumerable<GridDirection> directions,
                                        out IEnumerable<ICharacterEquippedItem> neighborItems) {
            if (!inventoryAggregate.tryGetNeighborItems(
                    sourceGridItemPlaced,
                    directions,
                    out IEnumerable<IInventoryPlacedItem> placedNeighbors)) {
                neighborItems = Enumerable.Empty<ICharacterEquippedItem>();
                return false;
            }

            neighborItems = mapToEquippedItems(placedNeighbors);
            return true;
        }

        public bool tryMoveItem(ICharacterEquippedItem itemToMove, Vector2Int newPosition) {
            return inventoryAggregate.tryChangeItemPosition(itemToMove.getId(), newPosition);
        }

        public IReadOnlyCollection<CharacterCombatTickableItemAction> getTickableItems() {
            tickableItemActions.Clear();

            foreach (IInventoryCombatTickableItem inventoryTickableItem in inventoryAggregate.getTickableItems()) {
                if (!tickableItemActionCache.TryGetValue(inventoryTickableItem,
                        out CharacterCombatTickableItemAction tickableItemAction)) {
                    tickableItemAction = (combatTicks, characterId, combatCapabilities) =>
                        inventoryTickableItem.tick(combatTicks, characterId, combatCapabilities);
                    tickableItemActionCache[inventoryTickableItem] = tickableItemAction;
                }

                tickableItemActions.Add(tickableItemAction);
            }

            return tickableItemActions;
        }

        private IReadOnlyCollection<ICharacterEquippedEntryPoint> mapToEquippedEntryPoints(
            IEnumerable<IInventoryPlacedEntryPoint> source) {
            return source
                .Select(ep => (ICharacterEquippedEntryPoint)getOrCreateCharacterEquippedItem(ep))
                .ToHashSet();
        }

        private IEnumerable<ICharacterEquippedItem> mapToEquippedItems(
            IEnumerable<IInventoryPlacedItem> placedItems) {
            return placedItems.Select(getOrCreateCharacterEquippedItem);
        }

        private ICharacterEquippedItem getOrCreateCharacterEquippedItem(IInventoryPlacedItem inventoryPlacedItem) {
            IInventoryPlacedItem placedItem = NullGuard.NotNullOrThrow(inventoryPlacedItem);
            Id<ItemId> itemId = placedItem.getId();

            if (equippedItemByItemId.TryGetValue(itemId, out ICharacterEquippedItem equippedItem)) {
                return equippedItem;
            }

            ICharacterEquippedItem newEquippedItem = placedItem switch {
                IInventoryPlacedEntryPoint inventoryPlacedEntryPoint =>
                    new CharacterEquippedEntryPointItem(inventoryPlacedEntryPoint),

                _ => new CharacterEquippedItem(placedItem)
            };

            equippedItemByItemId[itemId] = newEquippedItem;
            return newEquippedItem;
        }
    }
}