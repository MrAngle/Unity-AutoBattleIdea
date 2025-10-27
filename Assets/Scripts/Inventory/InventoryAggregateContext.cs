namespace Inventory {
    public class InventoryAggregateContext {
        private InventoryAggregate _inventoryAggregate;

        private InventoryAggregateContext(InventoryAggregate inventoryAggregate) {
            _inventoryAggregate = inventoryAggregate;
        }
        
        public static InventoryAggregateContext Create() {
            return new InventoryAggregateContext(InventoryAggregate.Create()); // for now
        }
        
        // public void SetInventoryGrid(IInventoryGrid inventoryGrid)
        // {
        //     _inventoryGrid = inventoryGrid;
        // }
        //
        public InventoryAggregate GetInventoryAggregate() {
            return _inventoryAggregate;
        }
    }
}