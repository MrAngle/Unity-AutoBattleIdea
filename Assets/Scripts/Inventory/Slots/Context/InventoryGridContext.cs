namespace Inventory.Slots.Context {
    public class InventoryGridContext : IInjected {
        private InventoryGrid _inventoryGrid;

        private InventoryGridContext() {}

        public static InventoryGridContext Create() {
            return new InventoryGridContext();
        }
        
        public void SetInventoryGrid(InventoryGrid inventoryGrid)
        {
            _inventoryGrid = inventoryGrid;
        }
        
        public InventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }
    }
}