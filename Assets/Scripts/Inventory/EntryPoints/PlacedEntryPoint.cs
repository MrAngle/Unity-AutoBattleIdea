using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.Flow.Domain.Aggregate;
using Combat.Flow.Domain.Router;
using Inventory.Items.Domain;
using Inventory.Position;
using Registry;
using Shared.Utility;
using UnityEngine;

namespace Inventory.EntryPoints {
    public class PlacedEntryPoint : IPlacedItem, IPlacedEntryPoint, IDisposable {
        // private readonly float _turnInterval; // sekundy
        // private readonly InventoryPosition _inventoryPosition;
        // private readonly FlowKind _kind;
        // private readonly Vector2Int _position;

        private bool _battleRunning = true; /*for now*/
        private CancellationTokenSource _cts;
        
        
        private readonly InventoryPosition _inventoryPosition;
        private readonly GridEntryPoint _entryPoint;
        private readonly IGridInspector _gridInspector;
        private readonly long _id;
        
        private PlacedEntryPoint(GridEntryPoint entryPoint, Vector2Int origin, IGridInspector gridInspector) {
            _id = IdGenerator.Next();
            _entryPoint = entryPoint;
            _gridInspector = gridInspector;
            
            _inventoryPosition = InventoryPosition.Create(origin, ItemShape.SingleCell());
        }
        
        public static IPlacedEntryPoint Create(FlowKind kind, Vector2Int position, IGridInspector gridInspector) {
            GridEntryPoint entryPoint = GridEntryPoint.Create(kind);
            PlacedEntryPoint placedEntryPoint = new PlacedEntryPoint(entryPoint, position, gridInspector);

            placedEntryPoint.StartBattle(); // for now
            return placedEntryPoint;
        }

        public void Process(FlowAggregate flowAggregate) {
            // DO nothing for now
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
        
        // public Vector2Int GetPosition() {
        //     return _position;
        // }

        // public static IEntryPointFacade Create(FlowKind kind, Vector2Int position) {
        //     GridEntryPoint entryPoint = new GridEntryPoint(kind, position);
        //     entryPoint.StartBattle(); // for now
        //     return entryPoint;
        // }

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
            // try {
                while (_battleRunning && !ct.IsCancellationRequested) {
                    await Task.Delay(TimeSpan.FromSeconds(_entryPoint.GetTurnInterval()), ct);

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
            // }
            // catch (TaskCanceledException) {
            //     // normalne wyjście po StopBattle()
            // }
        }

        private IFlowAggregateFacade PrepareFlowAggregate(int power) {
            IFlowRouter flowRouter = GridAdjacencyRouter.Create(_gridInspector);
            var flowAggregate = FlowAggregate.Create(this, power, flowRouter);
            return flowAggregate;
        }

        public override string ToString() {
            return $"({_entryPoint.GetFlowKind()})";
        }
    }
}