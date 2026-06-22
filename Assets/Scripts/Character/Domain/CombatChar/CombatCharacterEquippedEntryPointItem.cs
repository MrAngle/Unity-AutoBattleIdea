using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Character.Contract;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacterEquippedEntryPointItem : IFlowItem, IFlowPortPlacedItem {
        private readonly ICharacterEquippedEntryPoint characterEquippedEntryPointToTick;

        public CombatCharacterEquippedEntryPointItem(
            ICharacterEquippedEntryPoint characterEquippedEntryPointToTick) {
            this.characterEquippedEntryPointToTick = NullGuard.NotNullOrThrow(characterEquippedEntryPointToTick);
        }


        public Id<ItemId> getId() {
            return characterEquippedEntryPointToTick.getId();
        }

        public Vector2Int getOrigin() {
            return characterEquippedEntryPointToTick.getOrigin();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return characterEquippedEntryPointToTick.getOccupiedCells();
        }

        public ShapeArchetype getShape() {
            return characterEquippedEntryPointToTick.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return characterEquippedEntryPointToTick.prepareItemActionDescription();
        }

        public FlowPortKind getFlowPortKind() {
            return characterEquippedEntryPointToTick is IFlowPortPlacedItem portPlacedItem
                ? portPlacedItem.getFlowPortKind()
                : FlowPortKind.None;
        }

        public string getFlowPortName() {
            return characterEquippedEntryPointToTick is IFlowPortPlacedItem portPlacedItem
                ? portPlacedItem.getFlowPortName()
                : string.Empty;
        }

        public string getFlowPortDescription() {
            return characterEquippedEntryPointToTick is IFlowPortPlacedItem portPlacedItem
                ? portPlacedItem.getFlowPortDescription()
                : string.Empty;
        }
    }
}