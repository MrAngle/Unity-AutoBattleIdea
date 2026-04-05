using System;
using System.Collections.Generic;
using System.Threading;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Domain.ActionDescriptor;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.EntryPoint {
    internal class PlacedEntryPoint : IInventoryPlacedEntryPoint, IDisposable {
        private readonly IEntryPointArchetype entryPointArchetype;
        private readonly Id<ItemId> id;
        private readonly IInventoryPosition inventoryPosition;

        private CancellationTokenSource cancellationTokenSource;

        private bool isBattleRunning = true; /*for now*/

        private PlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition
        ) {
            id = new Id<ItemId>(IdGenerator.Next());
            this.entryPointArchetype = NullGuard.NotNullOrThrow(entryPointArchetype);
            this.inventoryPosition = NullGuard.NotNullOrThrow(inventoryPosition);
            NullGuard.NotNullCheckOrThrow(inventoryPosition);
        }

        public void Dispose() {
            stopBattle();
        }

        public IActionDescription prepareItemActionDescription() {
            IActionDescription actionSpecification = new ItemActionDescription(
                prepareCastTime(),
                prepareEffectsDescriptor());

            return actionSpecification;
        }

        public Vector2Int getOrigin() {
            return inventoryPosition.getOrigin();
        }

        public ShapeArchetype getShape() {
            return entryPointArchetype.getShape();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return inventoryPosition.getOccupiedCells();
        }

        public Id<ItemId> getId() {
            return id;
        }

        internal static IInventoryPlacedEntryPoint create(
            IEntryPointArchetype archetype,
            IInventoryPosition inventoryPosition
        ) {
            var placedEntryPoint =
                new PlacedEntryPoint(archetype, inventoryPosition);

            return placedEntryPoint;
        }

        private Duration prepareCastTime() {
            return new Duration(2f); // for now
        }

        private IOperations prepareEffectsDescriptor() {
            return new ItemOperationsDescription(
                new AddPower(new DamageToDeal(3))
            );
        }

        private void stopBattle() {
            if (!isBattleRunning) return;
            isBattleRunning = false;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        public override string ToString() {
            return $"({entryPointArchetype.getFlowKind()})";
        }
    }
}