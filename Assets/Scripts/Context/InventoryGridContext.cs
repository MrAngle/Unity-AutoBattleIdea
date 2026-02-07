using System;
using MageFactory.Character.Contract;

namespace MageFactory.Context {
    public class InventoryGridContext {
        private IInventoryGrid _inventoryGrid;

        public event Action<IInventoryGrid> InventoryGridChanged;

        public void setInventoryGrid(IInventoryGrid inventoryGrid) {
            if (_inventoryGrid == inventoryGrid)
                return; // should nothing to do

            _inventoryGrid = inventoryGrid;
            InventoryGridChanged?.Invoke(_inventoryGrid);
        }

        public IInventoryGrid getInventoryGrid() {
            return _inventoryGrid;
        }
    }
}