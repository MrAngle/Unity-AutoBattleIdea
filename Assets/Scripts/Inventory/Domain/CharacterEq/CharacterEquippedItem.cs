using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain.CharacterEq {
    internal class CharacterEquippedItem : ICharacterEquippedItem, IFlowPortPlacedItem {
        private IInventoryPlacedItem inventoryPlacedItem;

        public CharacterEquippedItem(IInventoryPlacedItem inventoryPlacedItem) {
            this.inventoryPlacedItem = NullGuard.NotNullOrThrow(inventoryPlacedItem);
        }

        public Id<ItemId> getId() {
            return inventoryPlacedItem.getId();
        }

        public Vector2Int getOrigin() {
            return inventoryPlacedItem.getOrigin();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return inventoryPlacedItem.getOccupiedCells();
        }

        public ShapeArchetype getShape() {
            return inventoryPlacedItem.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return inventoryPlacedItem.prepareItemActionDescription();
        }

        public FlowPortKind getFlowPortKind() {
            return inventoryPlacedItem is IFlowPortPlacedItem portPlacedItem
                ? portPlacedItem.getFlowPortKind()
                : FlowPortKind.None;
        }

        public string getFlowPortName() {
            return inventoryPlacedItem is IFlowPortPlacedItem portPlacedItem
                ? portPlacedItem.getFlowPortName()
                : string.Empty;
        }

        public string getFlowPortDescription() {
            return inventoryPlacedItem is IFlowPortPlacedItem portPlacedItem
                ? portPlacedItem.getFlowPortDescription()
                : string.Empty;
        }

        // internal IInventoryPlacedItem toInventoryPlacedItem() {
        //     return inventoryPlacedItem;
        // }
    }
}