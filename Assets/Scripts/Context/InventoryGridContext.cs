using System;
using Inventory.Slots.Domain;
using Inventory.Slots.Domain.Api;

namespace Inventory.Slots.Context {
    public class InventoryGridContext : IInjected {
        private IInventoryGrid _inventoryGrid;
        
        public event Action<IInventoryGrid> InventoryGridChanged;

        public InventoryGridContext() {}
        
        public void SetInventoryGrid(IInventoryGrid inventoryGrid)
        {
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