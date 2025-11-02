using Inventory.Items;
using Zenject;

namespace Inventory {
    public class InventoryAggregateContext {
        private readonly InventoryAggregate _inventoryAggregate;

        [Inject]
        public InventoryAggregateContext(IInventoryAggregateFactory inventoryAggregateFactory) {
            _inventoryAggregate = inventoryAggregateFactory.Create();
        }

        public InventoryAggregate GetInventoryAggregate() {
            return _inventoryAggregate;
        }
    }
}