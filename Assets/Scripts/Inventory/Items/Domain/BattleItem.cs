using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.ActionExecutor;
using Combat.Flow.Domain.Aggregate;
using Combat.Flow.Domain.Shared;
using Inventory.Position;
using Shared.Utility;
using TimeSystem;
using UI.Combat.Action;
using UnityEngine;

namespace Inventory.Items.Domain {

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

        public IActionSpecification GetAction() {
            ActionSpecification actionSpecification = new ActionSpecification(
                PrepareActionTiming(),
                PrepareActionCommandDescriptor());
            
            return actionSpecification;
        }
        
        private ActionTiming PrepareActionTiming() {
            return new ActionTiming(_itemArchetype.GetCastTime());
        } 

        private ActionCommandDescriptor PrepareActionCommandDescriptor() {
            return new ActionCommandDescriptor(
                new AddPower(new DamageToDeal(5))
            );
        } 

        public ShapeArchetype GetShape() {
            return _itemArchetype.GetShape();
        }

    }
}