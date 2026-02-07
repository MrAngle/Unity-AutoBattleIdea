using System;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.Context {
    public class InventoryAggregateContext {
        private readonly InventoryGridContext _inventoryGridContext;

        private ICharacterInventoryFacade _inventoryAggregate;

        public event Action<ICharacterInventoryFacade> OnInventoryAggregateSet;

        [Inject]
        public InventoryAggregateContext(IInventoryFactory inventoryFactory,
            InventoryGridContext inventoryGridContext) {
            // _inventoryAggregate = inventoryAggregateFactory.CreateCharacterInventory();
            _inventoryGridContext = NullGuard.NotNullOrThrow(inventoryGridContext);
        }

        public void setInventoryAggregateContext(ICharacterInventoryFacade inventoryAggregate) {
            if (_inventoryAggregate == inventoryAggregate)
                return; // should nothing to do

            _inventoryAggregate = inventoryAggregate;
            _inventoryGridContext.setInventoryGrid(_inventoryAggregate.getInventoryGrid());
            OnInventoryAggregateSet?.Invoke(_inventoryAggregate);
        }

        public ICharacterInventoryFacade getInventoryAggregateContext() {
            return _inventoryAggregate;
        }
    }
}