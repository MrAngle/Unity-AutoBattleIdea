using System;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.Context {
    public class InventoryAggregateContext {
        private readonly InventoryGridContext inventoryGridContext;

        private ICombatCharacterInventory characterInventory;
        public event Action<ICombatCharacterInventory> OnInventoryAggregateSet;

        [Inject]
        public InventoryAggregateContext(InventoryGridContext inventoryGridContext) {
            this.inventoryGridContext = NullGuard.NotNullOrThrow(inventoryGridContext);
        }

        public void setInventoryAggregateContext(ICombatCharacterInventory inventoryAggregate) {
            if (characterInventory == inventoryAggregate)
                return; // should nothing to do or clear

            characterInventory = inventoryAggregate;
            inventoryGridContext.setInventoryGrid(characterInventory.getInventoryGrid());
            OnInventoryAggregateSet?.Invoke(characterInventory);
        }
    }
}