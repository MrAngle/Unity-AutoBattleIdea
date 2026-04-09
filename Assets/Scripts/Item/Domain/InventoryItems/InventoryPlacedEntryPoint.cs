using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Domain.EntryPoint;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.InventoryItems {
    internal class InventoryPlacedEntryPoint : IInventoryPlacedEntryPoint {
        private readonly EntryPointItem entryPointItem;

        public InventoryPlacedEntryPoint(EntryPointItem entryPointItem) {
            this.entryPointItem = NullGuard.NotNullOrThrow(entryPointItem);
        }

        public Id<ItemId> getId() {
            return entryPointItem.getId();
        }

        public Vector2Int getOrigin() {
            return entryPointItem.getOrigin();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return entryPointItem.getOccupiedCells();
        }

        public ShapeArchetype getShape() {
            return entryPointItem.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return entryPointItem.prepareItemActionDescription();
        }

        public void updateItemPosition(IInventoryPosition inventoryPosition) {
            entryPointItem.updateItemPosition(inventoryPosition);
        }

        public FlowKind getFlowKind() {
            return entryPointItem.getFlowKind();
        }
    }
}