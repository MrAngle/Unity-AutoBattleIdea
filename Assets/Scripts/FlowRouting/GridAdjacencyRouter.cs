using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Contract;
using UnityEngine;

namespace MageFactory.FlowRouting {
    public class GridAdjacencyRouter : IFlowRouter {
        private readonly ICharacterCombatCapabilities
            characterCombatCapabilities; // it may be just query inspector instead of ICharacterCombatCapabilities

        private GridAdjacencyRouter(ICharacterCombatCapabilities characterCombatCapabilities) {
            this.characterCombatCapabilities = characterCombatCapabilities;
        }

        public ICombatCharacterEquippedItem decideNext(ICombatCharacterEquippedItem current,
            IReadOnlyCollection<long> visitedNodeIds) {
            // if (!_inventoryAggregate.TryGetItemAtCell(current.Position, out IPlacedItem placedItem)) {
            //     
            // }


            // 2) Zbierz sąsiednie kratki do całego shape’u ortogonalnie
            // var grid = _gridCtx.GetInventoryGrid();
            // if (grid == null) return null;
            // Debug.Log("Init DecideNext for flow");
            var dirs = new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            var boundary = new HashSet<Vector2Int>();

            foreach (var cell in current.getOccupiedCells())
            foreach (var d in dirs) {
                var n = cell + d;
                boundary.Add(n);
            }

            // 3) Kandydaci: kratki Occupied, należące do innego itemu
            // var candidates = new List<(ItemData item, Vector2Int origin, Vector2Int entryCell)>();
            var candidates = new Dictionary<Vector2Int, ICombatCharacterEquippedItem>();
            // Debug.Log("Candidates DecideNext for flow:" + $" {candidates}");
            foreach (var vector2Int in boundary)
                if (characterCombatCapabilities.query().tryGetItemAtCell(vector2Int, out var placedItem)) {
                    if (placedItem == current) continue; // ta sama bryła
                    // var neighborNodeId = $"Item:{neighborItem.Id}"; // TODO
                    if (visitedNodeIds.Contains(placedItem.getId())) continue; // już odwiedzony item
                    candidates.Add(vector2Int, placedItem);
                }

            // if (_itemIndex.TryGetItemAtCell(vector2Int, out var neighborItem, out var neighborOrigin))
            // {
            //     if (neighborItem == currentItem) continue; // ta sama bryła
            //     // var neighborNodeId = $"Item:{neighborItem.Id}"; // TODO
            //     if (visitedNodeIds.Contains(22 /*neighborNodeId*/)) continue; // już odwiedzony item
            //     candidates.Add((neighborItem, neighborOrigin, vector2Int));
            // }
            if (candidates.Count == 0)
                // Debug.Log("DecideNext - no candidates, return null");
                return null;
            // Debug.Log("Candidates Count DecideNext for flow:" + $" {candidates.Count}");


            var index = Random.Range(0, candidates.Count);
            var nextNodeToHandle = candidates.ElementAt(index).Value;

            // 4) Losuj jednego kandydata
            // var pick = candidates[_rng.Next(candidates.Count)];
            // var nextNode = new GridItemNode(pick.item, pick.origin);

            // Zwróć też kratkę wejścia (debug/telemetria)
            // return new RouteDecision(nextNodeToHandle, pick.entryCell);
            // Debug.Log("DecideNext - nextNodeToHandle:" + $" {nextNodeToHandle}");
            return nextNodeToHandle;
        }

        public static IFlowRouter create(ICharacterCombatCapabilities characterCombatCapabilities) {
            return new GridAdjacencyRouter(characterCombatCapabilities);
        }
    }
}