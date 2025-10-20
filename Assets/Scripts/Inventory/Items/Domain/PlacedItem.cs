using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Inventory.Items.Domain {
    /// <summary>
    /// Model przedmiotu umieszczonego w konkretnej siatce.
    /// </summary>
    public class PlacedItem
    {
        public ItemData Data { get; }
        public Vector2Int Origin { get; }

        public PlacedItem(ItemData data, Vector2Int origin)
        {
            Data = data;
            Origin = origin;
        }

        public IEnumerable<Vector2Int> OccupiedCells =>
            Data.Shape.Cells.Select(offset => Origin + offset);
    }
}