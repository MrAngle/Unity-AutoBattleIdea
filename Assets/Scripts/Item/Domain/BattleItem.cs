using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain {
    internal class BattleItem {
        private readonly Id<ItemId> id;
        private readonly ItemArchetype itemArchetype;
        private IInventoryPosition inventoryPosition;

        internal BattleItem(ItemArchetype itemArchetype, IInventoryPosition inventoryPosition) {
            id = new Id<ItemId>(IdGenerator.Next());
            this.itemArchetype = NullGuard.NotNullOrThrow(itemArchetype);
            this.inventoryPosition = NullGuard.NotNullOrThrow(inventoryPosition);
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return inventoryPosition.getOccupiedCells();
        }

        public Vector2Int getOrigin() {
            return inventoryPosition.getOrigin();
        }

        public Id<ItemId> getId() {
            return id;
        }

        public ShapeArchetype getShape() {
            return itemArchetype.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return itemArchetype.getActionDescription();
        }

        //
        // private Duration prepareActionTiming() {
        //     return new Duration(itemArchetype.getCastTime());
        // }
        //
        // private static IOperations prepareEffectsDescriptor() {
        //     return new ItemOperationsDescription(
        //         new AddPower(new DamageToDeal(5))
        //     );
        // }
        public void updateItemPosition(IInventoryPosition paramInventoryPosition) {
            inventoryPosition = paramInventoryPosition;
        }
    }
}