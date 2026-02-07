using System;
using MageFactory.Character.Contract;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.Context {
    public class InventoryAggregateContext {
        private readonly InventoryGridContext _inventoryGridContext;

        private ICharacterInventory characterInventory;

        public event Action<ICharacterInventory> OnInventoryAggregateSet;

        [Inject]
        public InventoryAggregateContext(IInventoryFactory inventoryFactory,
            InventoryGridContext inventoryGridContext) {
            // _inventoryAggregate = inventoryAggregateFactory.CreateCharacterInventory();
            _inventoryGridContext = NullGuard.NotNullOrThrow(inventoryGridContext);
        }

        public void setInventoryAggregateContext(ICharacterInventory inventoryAggregate) {
            if (characterInventory == inventoryAggregate)
                return; // should nothing to do or clear

            characterInventory = inventoryAggregate;
            _inventoryGridContext.setInventoryGrid(characterInventory.getInventoryGrid());
            OnInventoryAggregateSet?.Invoke(characterInventory);
        }

        public ICharacterInventory getInventoryAggregateContext() {
            return characterInventory;
        }
    }
}