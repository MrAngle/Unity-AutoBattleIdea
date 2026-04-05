using System.Collections.Generic;
using System.Linq;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
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

        public IInventoryGrid getInventoryGrid() {
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

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand) {
            return new CharacterEquippedItem(inventoryAggregate.place(placeItemCommand));
        }

        public bool canPlace(PlaceItemQuery placeItemCommand) {
            return inventoryAggregate.canPlace(placeItemCommand);
        }

        public HashSet<ICharacterEquippedEntryPointToTick> getEntryPointsToTick() {
            return inventoryAggregate.getEntryPointsToTick();
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

        private static IEnumerable<ICharacterEquippedItem> mapToEquippedItems(
            IEnumerable<IInventoryPlacedItem> placedItems) {
            return placedItems.Select(pi => (ICharacterEquippedItem)new CharacterEquippedItem(pi));
        }
    }
}