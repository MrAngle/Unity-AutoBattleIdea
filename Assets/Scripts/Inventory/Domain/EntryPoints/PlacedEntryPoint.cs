using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionEffect;
using MageFactory.Flow.Api;
using MageFactory.FlowRouting;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
// using MageFactory.Flow.Api;

namespace MageFactory.Inventory.Domain {
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
            StopBattle();
        }

        public IItemActionDescription prepareItemActionDescription() {
            IItemActionDescription actionSpecification = new ItemActionDescription(
                prepareCastTime(),
                prepareEffectsDescriptor());

            return actionSpecification;
        }

        // public IActionSpecification GetAction() {
        //     IActionSpecification actionSpecification = new ActionSpecification(
        //         PrepareActionTiming(),
        //         PrepareActionCommandDescriptor());
        //
        //     return actionSpecification;
        // }

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
            _ = BattleLoopAsync(_cts.Token); // fire-and-forget
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

        private IEffectsDescriptor prepareEffectsDescriptor() {
            return new EffectsDescription(
                new AddPower(new DamageToDeal(3))
            );
        }

        // private ActionTiming PrepareActionTiming() {
        //     return new ActionTiming(2f); // for now
        // }
        //
        // private IActionDescriptor PrepareActionCommandDescriptor() {
        //     return new ActionCommandDescriptor(
        //         new AddPower(new DamageToDeal(3))
        //     );
        // }

        public void StopBattle() {
            if (!_battleRunning) return;
            _battleRunning = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async Task BattleLoopAsync(CancellationToken ct) {
            while (_battleRunning && !ct.IsCancellationRequested) {
                await Task.Delay(TimeSpan.FromSeconds(_entryPointArchetype.GetTurnInterval()), ct);

                Debug.Log("Init proces for flow");
                if (ct.IsCancellationRequested || !_battleRunning) break;

                // var teamA = CharacterRegistry.Instance.GetTeamA();
                // var teamB = CharacterRegistry.Instance.GetTeamB();
                // if (teamA.Count == 0 || teamB.Count == 0) {
                //     Debug.Log("Brak postaci w drużynach — zatrzymuję walkę.");
                //     StopBattle();
                //     break;
                // }

                // wybierz losowe postacie (System.Random zamiast UnityEngine.Random)
                // var attacker = teamA[_rng.Next(0, teamA.Count)];
                // var target = teamB[_rng.Next(0, teamB.Count)];

                var power = 10;
                var flowAggregate = PrepareFlowAggregate(power);

                Debug.Log("Start proces for flow");
                flowAggregate.Start();
                // Debug.Log($"{attacker.Name} Start POWER: {power} to attack {target.Name}");
            }
        }

        private IFlowAggregateFacade PrepareFlowAggregate(int power) {
            var flowRouter = GridAdjacencyRouter.Create(_gridInspector);
            var flowAggregate = _flowFactory.Create(this, power, flowRouter);
            return flowAggregate;
        }

        public override string ToString() {
            return $"({_entryPointArchetype.GetFlowKind()})";
        }
    }
}