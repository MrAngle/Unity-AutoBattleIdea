using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class BattleItem : IPlacedItem {
        private readonly long _id;
        private readonly ItemArchetype _itemArchetype;
        private readonly InventoryPosition _inventoryPosition;

        internal BattleItem(ItemArchetype itemArchetype, Vector2Int origin) {
            _id = IdGenerator.Next();
            _itemArchetype = NullGuard.NotNullOrThrow(itemArchetype);
            _inventoryPosition = InventoryPosition.Create(origin, _itemArchetype.GetShape().Shape);
            // Register(Data, Origin);
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

        public IItemActionDescription prepareItemActionDescription() {
            IItemActionDescription actionSpecification = new ItemActionDescription(
                prepareActionTiming(),
                prepareEffectsDescriptor());

            return actionSpecification;
        }

        private Duration prepareActionTiming() {
            return new Duration(_itemArchetype.GetCastTime());
        }

        private IEffectsDescriptor prepareEffectsDescriptor() {
            return new EffectsDescription(
                new AddPower(new DamageToDeal(5))
            );
        }

        // private ActionTiming PrepareActionTiming() {
        //     return new ActionTiming(_itemArchetype.GetCastTime());
        // }
        //
        // private IActionDescriptor PrepareActionCommandDescriptor() {
        //     return new ActionCommandDescriptor(
        //         new AddPower(new DamageToDeal(5))
        //     );
        // }

        public ShapeArchetype GetShape() {
            return _itemArchetype.GetShape();
        }
    }
}