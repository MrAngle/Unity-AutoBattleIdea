using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.Flow.Domain;
using Combat.Flow.Domain.Aggregate;
using Inventory.Items.Domain;
using Inventory.Position;
using Registry;
using Shared.Utility;
using UnityEngine;
using Random = System.Random;

namespace Inventory.EntryPoints {
    public sealed class GridEntryPointId : StrongId<GridEntryPointId> {
    }

    public sealed class GridEntryPoint {
        private readonly float _turnInterval; // sekundy
        // private readonly InventoryPosition _inventoryPosition;
        private readonly FlowKind _kind;
        // private readonly Vector2Int _position;

        private bool _battleRunning;
        private CancellationTokenSource _cts;

        // prywatny ctor jak u Ciebie
        private GridEntryPoint(FlowKind kind) {
            _kind = kind;
            // _position = position;
            // _inventoryPosition = InventoryPosition.Create(position, ItemShape.SingleCell());
            _turnInterval = Mathf.Max(0.01f, 2.5f);
        }

        public static GridEntryPoint Create(FlowKind kind) {
            return new GridEntryPoint(kind);
        }

        public FlowKind GetFlowKind() {
            return _kind;
        }

        public float GetTurnInterval() {
            return _turnInterval;
        }
        
        // public void Dispose() {
        //     StopBattle();
        // }
        //
        // // public Vector2Int GetPosition() {
        // //     return _position;
        // // }
        //
        // // public static IEntryPointFacade Create(FlowKind kind, Vector2Int position) {
        // //     GridEntryPoint entryPoint = new GridEntryPoint(kind, position);
        // //     entryPoint.StartBattle(); // for now
        // //     return entryPoint;
        // // }
        //
        // public void StartBattle() {
        //     if (_battleRunning) return;
        //     _battleRunning = true;
        //     _cts = new CancellationTokenSource();
        //     _ = BattleLoopAsync(_cts.Token); // fire-and-forget
        // }
        //
        // public void StopBattle() {
        //     if (!_battleRunning) return;
        //     _battleRunning = false;
        //     _cts?.Cancel();
        //     _cts?.Dispose();
        //     _cts = null;
        // }
        //
        // private async Task BattleLoopAsync(CancellationToken ct) {
        //     try {
        //         while (_battleRunning && !ct.IsCancellationRequested) {
        //             await Task.Delay(TimeSpan.FromSeconds(_turnInterval), ct);
        //
        //             if (ct.IsCancellationRequested || !_battleRunning) break;
        //
        //             var teamA = CharacterRegistry.Instance.GetTeamA();
        //             var teamB = CharacterRegistry.Instance.GetTeamB();
        //             if (teamA.Count == 0 || teamB.Count == 0) {
        //                 Debug.Log("Brak postaci w drużynach — zatrzymuję walkę.");
        //                 StopBattle();
        //                 break;
        //             }
        //
        //             // wybierz losowe postacie (System.Random zamiast UnityEngine.Random)
        //             // var attacker = teamA[_rng.Next(0, teamA.Count)];
        //             // var target = teamB[_rng.Next(0, teamB.Count)];
        //
        //             var power = 10;
        //             var flowAggregate = FlowAggregate.Create(this, power);
        //             flowAggregate.Start();
        //             // Debug.Log($"{attacker.Name} Start POWER: {power} to attack {target.Name}");
        //         }
        //     }
        //     catch (TaskCanceledException) {
        //         // normalne wyjście po StopBattle()
        //     }
        // }

        public override string ToString() {
            return $"({_kind})";
        }
    }
}