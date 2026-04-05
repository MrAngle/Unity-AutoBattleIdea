using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MageFactory.Shared.Model.Shape {
    public class ItemShape {
        public IReadOnlyList<Vector2Int> Cells { get; }

        private ItemShape(IEnumerable<Vector2Int> cells) {
            var uniqueCells = cells.Distinct().ToList();

            if (uniqueCells.Count == 0) {
                throw new ArgumentException("Shape must contain at least one cell.");
            }

            if (!uniqueCells.Contains(Vector2Int.zero)) {
                throw new ArgumentException("Shape must contain (0,0) as origin cell.");
            }

            Cells = uniqueCells.AsReadOnly();
        }

        private IEnumerable<Vector2Int> getCellsAt(Vector2Int origin) {
            foreach (var off in Cells)
                yield return origin + off;
        }

        public HashSet<Vector2Int> getCellSetAt(Vector2Int origin) => new(getCellsAt(origin));

        public static ItemShape singleCell() => new(
            new[] { Vector2Int.zero }
        );

        public static ItemShape shapeL() =>
            new(new[] {
                Vector2Int.zero,
                new(1, 0),
                new(0, 1),
                new(0, 2)
            });

        public static ItemShape square2x2() =>
            new(new[] {
                Vector2Int.zero,
                new(1, 0),
                new(0, 1),
                new(1, 1)
            });

        public static ItemShape sword() =>
            new(new[] {
                Vector2Int.zero,
                new Vector2Int(0, 1),
                new Vector2Int(-1, 2),
                new Vector2Int(0, 2),
                new Vector2Int(1, 2),
                new Vector2Int(0, 3)
            });
    }
}