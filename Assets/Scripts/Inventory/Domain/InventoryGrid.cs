using MageFactory.Character.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    // probably should be removed
    internal class InventoryGrid : IReadOnlyInventoryGrid {
        private readonly InventoryRegistry inventoryRegistry;

        internal InventoryGrid(InventoryRegistry inventoryRegistry) {
            this.inventoryRegistry = NullGuard.NotNullOrThrow(inventoryRegistry);
        }

        public int getHeightCellsNumber() {
            return inventoryRegistry.getHeightCellsNumber();
        }

        public int getWidthCellsNumber() {
            return inventoryRegistry.getWidthCellsNumber();
        }

        public CellState getState(Vector2Int coord) {
            return inventoryRegistry.getCellState(coord);
        }
    }
}