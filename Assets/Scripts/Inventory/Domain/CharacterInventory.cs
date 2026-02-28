using System.Collections.Generic;
using System.Linq;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class CharacterInventory : ICharacterInventory {
        private InventoryAggregate inventoryAggregate;

        public CharacterInventory(InventoryAggregate inventoryAggregate) {
            this.inventoryAggregate = NullGuard.NotNullOrThrow(inventoryAggregate);
        }

        public IEnumerable<IGridItemPlaced> getPlacedSnapshot() {
            return inventoryAggregate.getPlacedSnapshot();
        }

        public ICombatInventory getInventoryGrid() {
            return inventoryAggregate.getInventoryGrid();
        }

        public bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item) {
            return inventoryAggregate.tryGetItemAtCell(cell, out item);
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
                                        out IEnumerable<ICharacterEquippedItem> neighborItems) {
            if (!inventoryAggregate.tryGetNeighborCells(
                    sourceGridItemPlaced,
                    out IEnumerable<IInventoryPlacedItem> placedNeighbors)) {
                neighborItems = Enumerable.Empty<ICharacterEquippedItem>();
                return false;
            }

            neighborItems = placedNeighbors;
            return true;
        }
    }
}