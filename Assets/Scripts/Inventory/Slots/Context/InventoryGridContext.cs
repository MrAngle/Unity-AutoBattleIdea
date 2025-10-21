namespace Inventory.Slots.Context {
    public class InventoryGridContext : IInjected {
        private InventoryGrid _inventoryGrid;
        // public bool IsReady { get; private set; }
        // public event Action<InventoryGrid> OnGridReady;

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
        
        // public bool TryGetInventoryGrid([NotNullWhen(true)] out InventoryGrid? grid)
        // {
        //     grid = _inventoryGrid;
        //     return grid is not null;
        // }
    }
}