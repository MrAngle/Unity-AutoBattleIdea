using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Flow.Api;
using MageFactory.FlowRouting;
using MageFactory.Inventory.Api;
using MageFactory.Inventory.Domain;
using MageFactory.Item.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Item.Domain {
    public class PlacedEntryPoint : IPlacedEntryPoint, IDisposable {
        private readonly IEntryPointArchetype _entryPointArchetype;
        private readonly IFlowFactory _flowFactory;
        private readonly IGridInspector _gridInspector;
        private readonly long _id;
        private readonly InventoryPosition _inventoryPosition;

        private bool _battleRunning = true; /*for now*/
        private CancellationTokenSource _cts;

        private PlacedEntryPoint(IEntryPointArchetype entryPointArchetype, Vector2Int origin,
            IGridInspector gridInspector, IFlowFactory flowFactory) {
            _id = IdGenerator.Next();
            _entryPointArchetype = NullGuard.NotNullOrThrow(entryPointArchetype);
            _gridInspector = NullGuard.NotNullOrThrow(gridInspector);
            _flowFactory = NullGuard.NotNullOrThrow(flowFactory);
            _inventoryPosition = InventoryPosition.Create(origin, ItemShape.SingleCell());
            NullGuard.NotNullCheckOrThrow(_inventoryPosition);
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


        public Vector2Int GetOrigin() {
            return _inventoryPosition.GetOrigin();
        }

        public ShapeArchetype GetShape() {
            return _entryPointArchetype.GetShape();
        }

        public IReadOnlyCollection<Vector2Int> GetOccupiedCells() {
            return _inventoryPosition.GetOccupiedCells();
        }

        public long GetId() {
            return _id;
        }

        public void StartBattle() {
            // if (_battleRunning) return;
            // _battleRunning = true;
            _cts = new CancellationTokenSource();
            _ = battleLoopAsync(_cts.Token); // fire-and-forget
        }

        internal static IPlacedEntryPoint Create(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector, IFlowFactory flowFactory) {
            var placedEntryPoint = new PlacedEntryPoint(archetype, position, gridInspector, flowFactory);

            placedEntryPoint.StartBattle(); // for now
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

        public void stopBattle() {
            if (!_battleRunning) return;
            _battleRunning = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async Task battleLoopAsync(CancellationToken ct) {
            while (_battleRunning && !ct.IsCancellationRequested) {
                await Task.Delay(TimeSpan.FromSeconds(_entryPointArchetype.GetTurnInterval()), ct);

                Debug.Log("Init proces for flow");
                if (ct.IsCancellationRequested || !_battleRunning) break;

                var power = 10;
                var flowAggregate = prepareFlowAggregate(power);

                Debug.Log("Start proces for flow");
                flowAggregate.start();
            }
        }

        private IFlowAggregateFacade prepareFlowAggregate(int power) {
            var flowRouter = GridAdjacencyRouter.Create(_gridInspector);
            var flowAggregate = _flowFactory.create(this, power, flowRouter);
            return flowAggregate;
        }

        public override string ToString() {
            return $"({_entryPointArchetype.GetFlowKind()})";
        }
    }
}