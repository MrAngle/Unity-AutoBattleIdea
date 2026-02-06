using System;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.Context {
    public class InventoryAggregateContext {
        private readonly InventoryGridContext _inventoryGridContext;

        private ICharacterInventoryFacade _inventoryAggregate;

        public event Action<ICharacterInventoryFacade> OnInventoryAggregateSet;

        [Inject]
        public InventoryAggregateContext(IInventoryAggregateFactory inventoryAggregateFactory,
            InventoryGridContext inventoryGridContext) {
            // _inventoryAggregate = inventoryAggregateFactory.CreateCharacterInventory();
            _inventoryGridContext = NullGuard.NotNullOrThrow(inventoryGridContext);
        }

        public void SetInventoryAggregateContext(ICharacterInventoryFacade inventoryAggregate) {
            if (_inventoryAggregate == inventoryAggregate)
                return; // should nothing to do

            _inventoryAggregate = inventoryAggregate;
            _inventoryGridContext.SetInventoryGrid(_inventoryAggregate.getInventoryGrid());
            OnInventoryAggregateSet?.Invoke(_inventoryAggregate);
        }

        public ICharacterInventoryFacade GetInventoryAggregateContext() {
            return _inventoryAggregate;
        }
    }
}