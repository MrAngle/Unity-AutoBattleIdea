using Inventory;
using Inventory.Items;
using Zenject;

namespace Context {
    public class InventoryAggregateContext {
        private ICharacterInventoryFacade _inventoryAggregate;

        [Inject]
        public InventoryAggregateContext(IInventoryAggregateFactory inventoryAggregateFactory) {
            // _inventoryAggregate = inventoryAggregateFactory.CreateCharacterInventory();
        }

        public void SetInventoryAggregateContext(ICharacterInventoryFacade inventoryAggregate) {
            _inventoryAggregate = inventoryAggregate;
        }
        
        public ICharacterInventoryFacade GetInventoryAggregateContext() {
            return _inventoryAggregate;
        }
    }
}