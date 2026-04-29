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
                item = new CharacterEquippedItem(inventoryPlacedItem);
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

            item = mapToCharacterEquippedItem(inventoryPlacedItem);
            return true;
        }

        public bool tryGetEntryPointById(Id<ItemId> itemId, out ICharacterEquippedEntryPoint entryPoint) {
            NullGuard.ValidIdOrThrow(itemId);

            if (inventoryAggregate.tryGetEntryPointById(itemId,
                    out IInventoryPlacedEntryPoint inventoryPlacedEntryPoint)) {
                entryPoint = new CharacterEquippedEntryPointItem(inventoryPlacedEntryPoint);
                return true;
            }

            entryPoint = null;
            return false;
        }

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand) {
            return new CharacterEquippedItem(inventoryAggregate.place(placeItemCommand));
        }

        public bool canPlace(PlaceItemQuery placeItemCommand) {
            return inventoryAggregate.canPlace(placeItemCommand);
        }

        public IReadOnlyCollection<ICharacterEquippedEntryPoint> getEntryPoints() {
            IReadOnlyCollection<IInventoryPlacedEntryPoint> aggregateSet = inventoryAggregate.getEntryPointsToTick();
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
            return inventoryAggregate.getTickableItems()
                .Select(inventoryTickableItem =>
                    (CharacterCombatTickableItemAction)((characterId, combatCapabilities) =>
                        inventoryTickableItem.tick(characterId, combatCapabilities)))
                .ToHashSet();
        }

        private static IReadOnlyCollection<ICharacterEquippedEntryPoint> mapToEquippedEntryPoints(
            IEnumerable<IInventoryPlacedEntryPoint> source) {
            return source
                .Select(ep => (ICharacterEquippedEntryPoint)new CharacterEquippedEntryPointItem(ep))
                .ToHashSet();
        }

        private static IEnumerable<ICharacterEquippedItem> mapToEquippedItems(
            IEnumerable<IInventoryPlacedItem> placedItems) {
            return placedItems.Select(pi => (ICharacterEquippedItem)new CharacterEquippedItem(pi));
        }

        private static ICharacterEquippedItem mapToCharacterEquippedItem(IInventoryPlacedItem inventoryPlacedItem) {
            return inventoryPlacedItem switch {
                IInventoryPlacedEntryPoint inventoryPlacedEntryPoint =>
                    new CharacterEquippedEntryPointItem(inventoryPlacedEntryPoint),

                _ => new CharacterEquippedItem(inventoryPlacedItem)
            };
        }
    }
}