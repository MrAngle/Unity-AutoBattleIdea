using System;
using Contracts.Inventory;

namespace Context {
    public class InventoryGridContext {
        private IInventoryGrid _inventoryGrid;

        public event Action<IInventoryGrid> InventoryGridChanged;

        public void SetInventoryGrid(IInventoryGrid inventoryGrid) {
            if (_inventoryGrid == inventoryGrid)
                return; // should nothing to do

            _inventoryGrid = inventoryGrid;
            InventoryGridChanged?.Invoke(_inventoryGrid);
        }

        public IInventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }
    }
}