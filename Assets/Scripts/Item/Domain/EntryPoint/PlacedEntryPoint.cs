using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Flow.Api;
using MageFactory.FlowRouting;
using MageFactory.Item.Api;
using MageFactory.Item.Controller.Api;
using MageFactory.Item.Domain.ActionDescriptor;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain.EntryPoint {
    internal class PlacedEntryPoint : IPlacedEntryPoint, IDisposable {
        private readonly long id;
        private readonly IEntryPointArchetype entryPointArchetype;
        private readonly IFlowFactory flowFactory;
        private readonly IGridInspector gridInspector;
        private readonly InventoryPosition inventoryPosition;

        private bool isBattleRunning = true; /*for now*/
        private CancellationTokenSource cancellationTokenSource;

        private PlacedEntryPoint(IEntryPointArchetype entryPointArchetype, Vector2Int origin,
            IGridInspector gridInspector, IFlowFactory flowFactory) {
            id = IdGenerator.Next();
            this.entryPointArchetype = NullGuard.NotNullOrThrow(entryPointArchetype);
            this.gridInspector = NullGuard.NotNullOrThrow(gridInspector);
            this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
            inventoryPosition = InventoryPosition.create(origin, ItemShape.SingleCell());
            NullGuard.NotNullCheckOrThrow(inventoryPosition);
        }

        internal static IPlacedEntryPoint create(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector, IFlowFactory flowFactory) {
            var placedEntryPoint = new PlacedEntryPoint(archetype, position, gridInspector, flowFactory);

            placedEntryPoint.startBattle(); // for now
            return placedEntryPoint;
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

        public void startBattle() {
            cancellationTokenSource = new CancellationTokenSource();
            _ = battleLoopAsync(cancellationTokenSource.Token); // fire-and-forget
        }

        private Duration prepareCastTime() {
            return new Duration(2f); // for now
        }

        private IOperations prepareEffectsDescriptor() {
            return new ItemOperationsDescription(
                new AddPower(new DamageToDeal(3))
            );
        }

        private async Task battleLoopAsync(CancellationToken ct) {
            while (isBattleRunning && !ct.IsCancellationRequested) {
                await Task.Delay(TimeSpan.FromSeconds(entryPointArchetype.getTurnInterval()), ct);

                Debug.Log("Init proces for flow");
                if (ct.IsCancellationRequested || !isBattleRunning) break;

                var power = 10;
                var flowAggregate = prepareFlowAggregate(power);

                Debug.Log("Start proces for flow");
                flowAggregate.start();
            }
        }

        private IFlowAggregateFacade prepareFlowAggregate(int power) {
            var flowRouter = GridAdjacencyRouter.create(gridInspector);
            var flowAggregate = flowFactory.create(this, power, flowRouter);
            return flowAggregate;
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