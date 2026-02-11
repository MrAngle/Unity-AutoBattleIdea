using System;
using MageFactory.CombatContext.Contract;

namespace MageFactory.Context {
    public class InventoryGridContext {
        private ICombatInventory _inventoryGrid;

        public event Action<ICombatInventory> InventoryGridChanged;

        public void setInventoryGrid(ICombatInventory inventoryGrid) {
            if (_inventoryGrid == inventoryGrid)
                return; // should nothing to do

            _inventoryGrid = inventoryGrid;
            InventoryGridChanged?.Invoke(_inventoryGrid);
        }

        public ICombatInventory getInventoryGrid() {
            return _inventoryGrid;
        }
    }
}