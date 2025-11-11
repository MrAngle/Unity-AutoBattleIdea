using Inventory;
using Inventory.Items;
using Zenject;

namespace Context {
    public class InventoryAggregateContext {
        private readonly ICharacterInventoryFacade _inventoryAggregate;

        [Inject]
        public InventoryAggregateContext(IInventoryAggregateFactory inventoryAggregateFactory) {
            _inventoryAggregate = inventoryAggregateFactory.CreateCharacterInventory();
        }

        public ICharacterInventoryFacade GetInventoryAggregate() {
            return _inventoryAggregate;
        }
    }
}