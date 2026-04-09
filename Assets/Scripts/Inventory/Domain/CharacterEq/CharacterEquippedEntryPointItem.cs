using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain.CharacterEq {
    internal class CharacterEquippedEntryPointItem : ICharacterEquippedEntryPointToTick {
        private readonly IInventoryPlacedEntryPoint inventoryPlacedEntryPoint;

        public CharacterEquippedEntryPointItem(IInventoryPlacedEntryPoint inventoryPlacedEntryPoint) {
            this.inventoryPlacedEntryPoint = NullGuard.NotNullOrThrow(inventoryPlacedEntryPoint);
        }

        public Id<ItemId> getId() {
            return inventoryPlacedEntryPoint.getId();
        }

        public Vector2Int getOrigin() {
            return inventoryPlacedEntryPoint.getOrigin();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return inventoryPlacedEntryPoint.getOccupiedCells();
        }

        public ShapeArchetype getShape() {
            return inventoryPlacedEntryPoint.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return inventoryPlacedEntryPoint.prepareItemActionDescription();
        }

        public FlowKind getFlowKind() {
            return inventoryPlacedEntryPoint.getFlowKind();
        }
    }
}