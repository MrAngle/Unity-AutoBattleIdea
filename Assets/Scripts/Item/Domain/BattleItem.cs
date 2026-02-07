using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Inventory.Api;
using MageFactory.Inventory.Domain;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item {
    internal class BattleItem : IPlacedItem {
        private readonly long _id;
        private readonly ItemArchetype _itemArchetype;
        private readonly InventoryPosition _inventoryPosition;

        internal BattleItem(ItemArchetype itemArchetype, Vector2Int origin) {
            _id = IdGenerator.Next();
            _itemArchetype = NullGuard.NotNullOrThrow(itemArchetype);
            _inventoryPosition = InventoryPosition.Create(origin, _itemArchetype.GetShape().Shape);
        }

        public IReadOnlyCollection<Vector2Int> GetOccupiedCells() {
            return _inventoryPosition.GetOccupiedCells();
        }

        public Vector2Int GetOrigin() {
            return _inventoryPosition.GetOrigin();
        }

        public long GetId() {
            return _id;
        }

        public IActionDescription prepareItemActionDescription() {
            IActionDescription actionSpecification = new ItemActionDescription(
                prepareActionTiming(),
                prepareEffectsDescriptor());

            return actionSpecification;
        }

        private Duration prepareActionTiming() {
            return new Duration(_itemArchetype.getCastTime());
        }

        private IOperations prepareEffectsDescriptor() {
            return new ItemOperationsDescription(
                new AddPower(new DamageToDeal(5))
            );
        }

        public ShapeArchetype GetShape() {
            return _itemArchetype.GetShape();
        }
    }
}