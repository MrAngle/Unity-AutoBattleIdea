using Inventory.Slots.Domain;
using Inventory.Slots.Domain.Api;

namespace Inventory.Slots.Context {
    public class InventoryGridContext : IInjected {
        private IInventoryGrid _inventoryGrid;

        public InventoryGridContext() {}
        
        public void SetInventoryGrid(IInventoryGrid inventoryGrid)
        {
            _inventoryGrid = inventoryGrid;
        }
        
        public IInventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }
    }
}