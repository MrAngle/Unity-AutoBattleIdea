using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.InventoryItems {
    internal class InventoryPlacedBattleItem : IInventoryPlacedItem {
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

        public void updateItemPosition(IInventoryPosition inventoryPosition) {
            battleItem.updateItemPosition(inventoryPosition);
        }
    }
}