using System;
using System.Collections.Generic;
using System.Threading;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Domain.ActionDescriptor;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.EntryPoint {
    internal class PlacedEntryPoint : IInventoryPlacedEntryPoint, IDisposable {
        private readonly IEntryPointArchetype entryPointArchetype;

        private readonly long id;

        // private readonly IFlowFactory flowFactory;
        private readonly IInventoryPosition inventoryPosition;
        // private readonly ICharacterCombatCapabilities characterCombatCapabilities;

        private CancellationTokenSource cancellationTokenSource;

        private bool isBattleRunning = true; /*for now*/

        private PlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition
            // IFlowFactory flowFactory,
            // ICharacterCombatCapabilities characterCombatCapabilities
        ) {
            id = IdGenerator.Next();
            this.entryPointArchetype = NullGuard.NotNullOrThrow(entryPointArchetype);
            // this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
            this.inventoryPosition = NullGuard.NotNullOrThrow(inventoryPosition);
            // this.characterCombatCapabilities = NullGuard.NotNullOrThrow(characterCombatCapabilities);
            // this.inventoryPosition = InventoryPosition.create(origin, ItemShape.SingleCell());
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

        public long getId() {
            return id;
        }

        internal static IInventoryPlacedEntryPoint create(
            IEntryPointArchetype archetype,
            IInventoryPosition inventoryPosition
            // IFlowFactory flowFactory,
            // ICharacterCombatCapabilities characterCombatCapabilities
        ) {
            var placedEntryPoint =
                new PlacedEntryPoint(archetype, inventoryPosition /*, flowFactory, characterCombatCapabilities*/);

            // placedEntryPoint.startBattle(); // for now
            return placedEntryPoint;
        }

        // public void startBattle() {
        //     cancellationTokenSource = new CancellationTokenSource();
        //     _ = battleLoopAsync(cancellationTokenSource.Token); // fire-and-forget
        // }

        private Duration prepareCastTime() {
            return new Duration(2f); // for now
        }

        private IOperations prepareEffectsDescriptor() {
            return new ItemOperationsDescription(
                new AddPower(new DamageToDeal(3))
            );
        }

        // private async Task battleLoopAsync(CancellationToken ct) {
        //     while (isBattleRunning && !ct.IsCancellationRequested) {
        //         await Task.Delay(TimeSpan.FromSeconds(entryPointArchetype.getTurnInterval()), ct);
        //
        //         Debug.Log("Init proces for flow");
        //         if (ct.IsCancellationRequested || !isBattleRunning) break;
        //
        //         var flowAggregate = prepareFlowAggregate();
        //
        //         Debug.Log("Start proces for flow");
        //         flowAggregate.start();
        //     }
        // }

        // private IFlowAggregateFacade prepareFlowAggregate() {
        //     var flowRouter = GridAdjacencyRouter.create(characterCombatCapabilities);
        //     var flowAggregate = flowFactory.create(this, flowRouter);
        //     return flowAggregate;
        // }

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