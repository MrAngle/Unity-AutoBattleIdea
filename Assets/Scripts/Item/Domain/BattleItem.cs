using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Domain.ActionDescriptor;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain {
    internal class BattleItem : IInventoryPlacedItem {
        private readonly long id;
        private readonly ItemArchetype itemArchetype;
        private readonly IInventoryPosition inventoryPosition;

        internal BattleItem(ItemArchetype itemArchetype, IInventoryPosition inventoryPosition) {
            id = IdGenerator.Next();
            this.itemArchetype = NullGuard.NotNullOrThrow(itemArchetype);
            this.inventoryPosition = NullGuard.NotNullOrThrow(inventoryPosition);
            // _inventoryPosition = InventoryPosition.create(origin, _itemArchetype.getShape().Shape);
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return inventoryPosition.getOccupiedCells();
        }

        public Vector2Int getOrigin() {
            return inventoryPosition.getOrigin();
        }

        public long getId() {
            return id;
        }

        public ShapeArchetype getShape() {
            return itemArchetype.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            IActionDescription actionSpecification = new ItemActionDescription(
                prepareActionTiming(),
                prepareEffectsDescriptor());

            return actionSpecification;
        }

        private Duration prepareActionTiming() {
            return new Duration(itemArchetype.getCastTime());
        }

        private static IOperations prepareEffectsDescriptor() {
            return new ItemOperationsDescription(
                new AddPower(new DamageToDeal(5))
            );
        }
    }
}