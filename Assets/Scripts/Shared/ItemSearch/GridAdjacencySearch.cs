using System.Collections.Generic;
using System.Linq;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using UnityEngine;

namespace MageFactory.Shared.ItemSearch {
    public static class GridAdjacencySearch {
        public delegate bool TryGetItemAtCell(Vector2Int cell, out IGridItemPlaced item);

        public static IEnumerable<T> getNeighborItems<T>(
            IGridItemPlaced sourceItem,
            IReadOnlyDictionary<Vector2Int, T> cellIndex,
            IEnumerable<GridDirection> directions)
            where T : IGridItemPlaced {
            if (sourceItem == null)
                yield break;

            var gridDirections = directions as GridDirection[]
                                 ?? directions.ToArray();

            var seenItems = new HashSet<long>();

            foreach (var cell in sourceItem.getOccupiedCells()) {
                foreach (var gridDirection in gridDirections) {
                    if (gridDirection == GridDirection.None)
                        continue;

                    var neighborCell = cell + gridDirection.toVector2Int();

                    if (!cellIndex.TryGetValue(neighborCell, out var neighbor))
                        continue;

                    if (neighbor.getId() == sourceItem.getId())
                        continue;

                    if (!seenItems.Add(neighbor.getId()))
                        continue;

                    yield return neighbor;
                }
            }
        }

        public static bool tryGetNeighborItem<T>(
            T sourceItem,
            IReadOnlyDictionary<Vector2Int, T> cellIndex,
            IEnumerable<GridDirection> directions,
            out T neighbor)
            where T : IGridItemPlaced {
            foreach (var n in getNeighborItems(sourceItem, cellIndex, directions)) {
                neighbor = n;
                return true;
            }

            neighbor = default;
            return false;
        }
    }
}