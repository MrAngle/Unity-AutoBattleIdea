
using System.Collections.Generic;
using System.Linq;
using Combat.Flow.Domain.Aggregate;
using Inventory.Slots;
using Inventory.Slots.Context;
using Inventory.Slots.Domain.Api;
using Inventory.Items.Domain;
using UnityEngine;
using Zenject;

namespace Combat.Flow.Domain.Router
{
    public class GridAdjacencyRouter : IFlowRouter
    {
        private readonly InventoryGridContext _gridCtx;
        private readonly IGridItemIndex _index;
        private readonly System.Random _rng;

        [Inject]
        public GridAdjacencyRouter(InventoryGridContext gridCtx, IGridItemIndex index, System.Random rng)
        {
            _gridCtx = gridCtx;
            _index = index;
            _rng = rng;
        }

        public RouteDecision? DecideNext(IFlowNode current, FlowModel model, IReadOnlyCollection<long> visitedNodeIds)
        {
            // 1) Ustal “bieżący item” albo go znajdź na starcie
            ItemData currentItem;
            Vector2Int currentOrigin;

            if (current is GridItemNode itemNode)
            {
                currentItem = itemNode.Item;
                currentOrigin = itemNode.Origin;
            }
            else if (current is GridStartNode startNode)
            {
                // Start — spróbuj znaleźć item w komórce startowej
                if (!_index.TryGetItemAtCell(startNode.Position, out currentItem, out currentOrigin))
                    return null; // brak itemu na starcie => koniec
            }
            else
            {
                return null;
            }

            // 2) Zbierz sąsiednie kratki do całego shape’u ortogonalnie
            var grid = _gridCtx.GetInventoryGrid();
            if (grid == null) return null;

            var dirs = new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            var boundary = new HashSet<Vector2Int>();

            foreach (var cell in _index.GetOccupiedCells(currentItem, currentOrigin))
            {
                foreach (var d in dirs)
                {
                    var n = cell + d;
                    boundary.Add(n);
                }
            }

            // 3) Kandydaci: kratki Occupied, należące do innego itemu
            var candidates = new List<(ItemData item, Vector2Int origin, Vector2Int entryCell)>();
            foreach (var n in boundary)
            {
                if (grid.GetState(n) != CellState.Occupied) {
                    continue;
                }
                if (_index.TryGetItemAtCell(n, out var neighborItem, out var neighborOrigin))
                {
                    if (neighborItem == currentItem) continue; // ta sama bryła
                    // var neighborNodeId = $"Item:{neighborItem.Id}"; // TODO
                    if (visitedNodeIds.Contains(22 /*neighborNodeId*/)) continue; // już odwiedzony item
                    candidates.Add((neighborItem, neighborOrigin, n));
                }
            }

            if (candidates.Count == 0)
                return null;

            // 4) Losuj jednego kandydata
            var pick = candidates[_rng.Next(candidates.Count)];
            var nextNode = new GridItemNode(pick.item, pick.origin);

            // Zwróć też kratkę wejścia (debug/telemetria)
            return new RouteDecision(nextNode, pick.entryCell);
        }
    }
}
