using Inventory.Slots.Domain;
using Inventory.Slots.Domain.Api;

namespace Inventory.Slots.Context {
    public class InventoryGridContext : IInjected {
        private IInventoryGrid _inventoryGrid;
        private InventoryAggregate inventoryAggregate;

        private InventoryGridContext() {}

        public static InventoryGridContext Create() {
            return new InventoryGridContext();
        }
        
        public InventoryAggregate CreateNewContext() {
            return InventoryAggregate.Create();
        }
        
        public void SetInventoryGrid(IInventoryGrid inventoryGrid)
        {
            _inventoryGrid = inventoryGrid;
        }
        
        public IInventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }
    }
}