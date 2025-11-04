using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.ActionExecutor;
using Combat.Flow.Domain.Aggregate;
using Combat.Flow.Domain.Router;
using Inventory.Items.Domain;
using Inventory.Position;
using Registry;
using Shared.Utility;
using TimeSystem;
using UI.Combat.Action;
using UnityEngine;
using Zenject;

namespace Inventory.EntryPoints {
    public class PlacedEntryPoint : IPlacedEntryPoint, IDisposable {
        // private readonly float _turnInterval; // sekundy
        // private readonly InventoryPosition _inventoryPosition;
        // private readonly FlowKind _kind;
        // private readonly Vector2Int _position;
        
        private readonly IFlowFactory _flowFactory;

        private bool _battleRunning = true; /*for now*/
        private CancellationTokenSource _cts;
        
        
        private readonly InventoryPosition _inventoryPosition;
        private readonly EntryPointArchetype _entryPointArchetype;
        private readonly IGridInspector _gridInspector;
        private readonly long _id;
        
        private PlacedEntryPoint(EntryPointArchetype entryPointArchetype, Vector2Int origin, IGridInspector gridInspector, IFlowFactory flowFactory) {
            _id = IdGenerator.Next();
            _entryPointArchetype = entryPointArchetype;
            _gridInspector = gridInspector;
            _flowFactory = flowFactory;
            _inventoryPosition = InventoryPosition.Create(origin, ItemShape.SingleCell());
        }
        
        internal static IPlacedEntryPoint Create(EntryPointArchetype archetype, Vector2Int position, IGridInspector gridInspector, IFlowFactory flowFactory) {
            PlacedEntryPoint placedEntryPoint = new PlacedEntryPoint(archetype, position, gridInspector, flowFactory);

            placedEntryPoint.StartBattle(); // for now
            return placedEntryPoint;
        }
        
        public IActionSpecification GetAction() {
            ActionSpecification actionSpecification = new ActionSpecification(
                PrepareActionTiming(),
                PrepareActionCommandDescriptor());
            
            return actionSpecification;
        }
        
        private ActionTiming PrepareActionTiming() {
            return new ActionTiming(2f); // for now
        } 

        private ActionCommandDescriptor PrepareActionCommandDescriptor() {
            return new ActionCommandDescriptor(
                new AddPower(3)
            );
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

       public void Dispose() {
            StopBattle();
        }

        public void StartBattle() {
            // if (_battleRunning) return;
            // _battleRunning = true;
            _cts = new CancellationTokenSource();
            _ = BattleLoopAsync(_cts.Token); // fire-and-forget
        }

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

                var teamA = CharacterRegistry.Instance.GetTeamA();
                var teamB = CharacterRegistry.Instance.GetTeamB();
                if (teamA.Count == 0 || teamB.Count == 0) {
                    Debug.Log("Brak postaci w drużynach — zatrzymuję walkę.");
                    StopBattle();
                    break;
                }

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
            IFlowRouter flowRouter = GridAdjacencyRouter.Create(_gridInspector);
            IFlowAggregateFacade flowAggregate = _flowFactory.Create(this, power, flowRouter);
            return flowAggregate;
        }

        public override string ToString() {
            return $"({_entryPointArchetype.GetFlowKind()})";
        }
    }
}