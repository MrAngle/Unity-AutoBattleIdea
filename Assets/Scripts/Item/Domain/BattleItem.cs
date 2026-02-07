using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Item.Controller.Api;
using MageFactory.Item.Domain.ActionDescriptor;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain {
    internal class BattleItem : IPlacedItem {
        private readonly long _id;
        private readonly ItemArchetype _itemArchetype;
        private readonly InventoryPosition _inventoryPosition;

        internal BattleItem(ItemArchetype itemArchetype, Vector2Int origin) {
            _id = IdGenerator.Next();
            _itemArchetype = NullGuard.NotNullOrThrow(itemArchetype);
            _inventoryPosition = InventoryPosition.create(origin, _itemArchetype.getShape().Shape);
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return _inventoryPosition.getOccupiedCells();
        }

        public Vector2Int getOrigin() {
            return _inventoryPosition.getOrigin();
        }

        public long getId() {
            return _id;
        }

        public ShapeArchetype getShape() {
            return _itemArchetype.getShape();
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
    }
}