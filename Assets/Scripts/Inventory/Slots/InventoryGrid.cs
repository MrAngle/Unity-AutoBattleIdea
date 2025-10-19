using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Slots {
    public class InventoryGrid {
        private readonly Dictionary<Vector2Int, InventoryCell> _cells;

        public InventoryGrid(int width, int height)
        {
            _cells = new Dictionary<Vector2Int, InventoryCell>();
            for (int xIndex = 0; xIndex < width; xIndex++)
            {
                for (int yIndex = 0; yIndex < height; yIndex++)
                {
                    _cells[new Vector2Int(xIndex, yIndex)] = new InventoryCell();
                }
            }
        }

        public InventoryCell GetCell(Vector2Int pos)
        {
            _cells.TryGetValue(pos, out var cell);
            return cell;
        }

        // public bool CanPlace(ItemShape shape, Vector2Int origin)
        // {
        //     foreach (var offset in shape.Cells)
        //     {
        //         var pos = origin + offset;
        //         if (!_cells.ContainsKey(pos)) return false;
        //         if (_cells[pos].State != CellState.Empty) return false;
        //     }
        //     return true;
        // }
    }
}