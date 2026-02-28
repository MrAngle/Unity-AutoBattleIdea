using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class CharacterEquippedItem : ICharacterEquippedItem {
        private IInventoryPlacedItem inventoryPlacedItem;

        public CharacterEquippedItem(IInventoryPlacedItem inventoryPlacedItem) {
            this.inventoryPlacedItem = NullGuard.NotNullOrThrow(inventoryPlacedItem);
        }

        public long getId() {
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

        internal IInventoryPlacedItem toInventoryPlacedItem() {
            return inventoryPlacedItem;
        }
    }
}