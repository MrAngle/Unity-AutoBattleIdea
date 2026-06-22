using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.CombatEvents;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.EntryPoint {
    internal class EntryPointItem {
        private readonly Id<ItemId> id;
        private readonly IEntryPointArchetype entryPointArchetype;
        private IInventoryPosition inventoryPosition;

        private EntryPointItem(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition
        ) {
            id = new Id<ItemId>(IdGenerator.Next());
            this.entryPointArchetype = NullGuard.NotNullOrThrow(entryPointArchetype);
            this.inventoryPosition = NullGuard.NotNullOrThrow(inventoryPosition);
            NullGuard.NotNullCheckOrThrow(this.inventoryPosition);
        }

        internal static EntryPointItem create(
            IEntryPointArchetype archetype,
            IInventoryPosition inventoryPosition
        ) {
            var placedEntryPoint =
                new EntryPointItem(archetype, inventoryPosition);

            return placedEntryPoint;
        }

        public Id<ItemId> getId() {
            return id;
        }

        public Vector2Int getOrigin() {
            return inventoryPosition.getOrigin();
        }

        public CombatTicks getTriggerInterval() {
            return entryPointArchetype.getTriggerInterval();
        }

        public EntryPointTriggerKind getTriggerKind() {
            return entryPointArchetype.getTriggerKind();
        }

        public ICombatHook getCombatHook() {
            return entryPointArchetype.getCombatHook();
        }

        public ShapeArchetype getShape() {
            return entryPointArchetype.getItemDefinition().getShape();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return inventoryPosition.getOccupiedCells();
        }

        public FlowKind getFlowKind() {
            return entryPointArchetype.getFlowKind();
        }

        public IActionDescription prepareItemActionDescription() {
            return entryPointArchetype.getItemDefinition().getActionDescription();
        }

        public FlowPortKind getFlowPortKind() {
            return entryPointArchetype.getItemDefinition() is IFlowPortDefinition portDefinition
                ? portDefinition.getFlowPortKind()
                : FlowPortKind.None;
        }

        public string getFlowPortName() {
            return entryPointArchetype.getItemDefinition() is IFlowPortDefinition portDefinition
                ? portDefinition.getFlowPortName()
                : string.Empty;
        }

        public string getFlowPortDescription() {
            return entryPointArchetype.getItemDefinition() is IFlowPortDefinition portDefinition
                ? portDefinition.getFlowPortDescription()
                : string.Empty;
        }

        // think about this - item should never be able to move itselv
        public void updateItemPosition(IInventoryPosition paramInventoryPosition) {
            inventoryPosition = paramInventoryPosition;
        }

        public override string ToString() {
            return $"({entryPointArchetype.getFlowKind()})";
        }
    }
}