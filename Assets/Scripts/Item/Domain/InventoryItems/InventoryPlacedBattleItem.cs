using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.InventoryItems {
    internal class InventoryPlacedBattleItem : IInventoryPlacedItem, IFlowPortPlacedItem {
        private readonly BattleItem battleItem;

        public InventoryPlacedBattleItem(BattleItem battleItem) {
            this.battleItem = NullGuard.NotNullOrThrow(battleItem);
        }

        public Id<ItemId> getId() {
            return battleItem.getId();
        }

        public Vector2Int getOrigin() {
            return battleItem.getOrigin();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return battleItem.getOccupiedCells();
        }

        public ShapeArchetype getShape() {
            return battleItem.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return battleItem.prepareItemActionDescription();
        }

        public FlowPortKind getFlowPortKind() {
            return battleItem.getFlowPortKind();
        }

        public string getFlowPortName() {
            return battleItem.getFlowPortName();
        }

        public string getFlowPortDescription() {
            return battleItem.getFlowPortDescription();
        }

        public void updateItemPosition(IInventoryPosition inventoryPosition) {
            battleItem.updateItemPosition(inventoryPosition);
        }
    }
}