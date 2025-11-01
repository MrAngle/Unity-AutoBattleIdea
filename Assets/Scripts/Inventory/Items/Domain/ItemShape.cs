using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Items.Domain {
    /// <summary>
    /// Reprezentuje kształt przedmiotu w układzie siatki.
    /// Pozycja (0,0) to punkt odniesienia (origin) przy umieszczaniu w siatce.
    /// </summary>
    public class ItemShape
    {
        public IReadOnlyList<Vector2Int> Cells { get; }

        public ItemShape(IEnumerable<Vector2Int> cells)
        {
            Cells = cells.ToList().AsReadOnly();
        }
        
        public IEnumerable<Vector2Int> GetCellsAt(Vector2Int origin)
        {
            foreach (var off in Cells)
                yield return origin + off;
        }

        public HashSet<Vector2Int> GetCellSetAt(Vector2Int origin) => new(GetCellsAt(origin));

        public static ItemShape SingleCell() =>
            new ItemShape(new[] { Vector2Int.zero });

        public static ItemShape LShape() =>
            new ItemShape(new[] {
                Vector2Int.zero,
                new(1, 0),
                new(0, 1),
                new(0, 2)
            });

        public static ItemShape Square2x2() =>
            new ItemShape(new[] {
                Vector2Int.zero,
                new(1, 0),
                new(0, 1),
                new(1, 1)
            });
    }
}