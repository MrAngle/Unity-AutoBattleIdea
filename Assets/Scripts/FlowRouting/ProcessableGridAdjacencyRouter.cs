using System.Collections.Generic;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.FlowRouting {
    public sealed class ProcessableGridAdjacencyRouter : IFlowRouter {
        private static readonly Vector2Int[] Directions = {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        private readonly TryGetFlowItemAtCell tryGetItemAtCell;
        private readonly CanProcessFlowItem canProcessFlowItem;
        private readonly HashSet<Vector2Int> boundaryCells = new();
        private readonly List<IFlowItem> candidates = new();

        private ProcessableGridAdjacencyRouter(
            TryGetFlowItemAtCell tryGetItemAtCell,
            CanProcessFlowItem canProcessFlowItem) {
            this.tryGetItemAtCell = NullGuard.NotNullOrThrow(tryGetItemAtCell);
            this.canProcessFlowItem = NullGuard.NotNullOrThrow(canProcessFlowItem);
        }

        public static IFlowRouter create(
            TryGetFlowItemAtCell tryGetItemAtCell,
            CanProcessFlowItem canProcessFlowItem) {
            return new ProcessableGridAdjacencyRouter(tryGetItemAtCell, canProcessFlowItem);
        }

        public IFlowItem decideNext(
            IGridItemPlaced current,
            IReadOnlyCollection<Id<ItemId>> visitedNodeIds) {
            NullGuard.NotNullOrThrow(current);
            NullGuard.NotNullOrThrow(visitedNodeIds);

            boundaryCells.Clear();
            candidates.Clear();

            foreach (Vector2Int cell in current.getOccupiedCells())
            foreach (Vector2Int direction in Directions) {
                boundaryCells.Add(cell + direction);
            }

            foreach (Vector2Int cell in boundaryCells) {
                if (!tryGetItemAtCell(cell, out IFlowItem placedItem)) {
                    continue;
                }

                if (placedItem.getId().Equals(current.getId())) {
                    continue;
                }

                if (containsVisitedItem(visitedNodeIds, placedItem.getId())) {
                    continue;
                }

                if (!canProcessFlowItem(placedItem)) {
                    continue;
                }

                if (containsCandidate(placedItem.getId())) {
                    continue;
                }

                candidates.Add(placedItem);
            }

            if (candidates.Count == 0) {
                return null;
            }

            int index = Random.Range(0, candidates.Count);
            return candidates[index];
        }

        private static bool containsVisitedItem(
            IReadOnlyCollection<Id<ItemId>> visitedNodeIds,
            Id<ItemId> itemId) {
            if (visitedNodeIds is HashSet<Id<ItemId>> visitedNodeIdSet) {
                return visitedNodeIdSet.Contains(itemId);
            }

            foreach (Id<ItemId> visitedNodeId in visitedNodeIds) {
                if (visitedNodeId.Equals(itemId)) {
                    return true;
                }
            }

            return false;
        }

        private bool containsCandidate(Id<ItemId> itemId) {
            for (int i = 0; i < candidates.Count; i++) {
                if (candidates[i].getId().Equals(itemId)) {
                    return true;
                }
            }

            return false;
        }
    }
}