using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Slots {
    public class InventoryGrid {
        private readonly Dictionary<Vector2Int, InventoryCell> _cells;
        private readonly int _width;
        private readonly int _height;
        
        public int Width => _width;
        public int Height => _height;
        

        public InventoryGrid(int width, int height)
        {
            _width = Mathf.Max(0, width);
            _height = Mathf.Max(0, height);

            _cells = new Dictionary<Vector2Int, InventoryCell>();
            for (int xIndex = 0; xIndex < _width; xIndex++)
            {
                for (int yIndex = 0; yIndex < _height; yIndex++)
                {
                    _cells[new Vector2Int(xIndex, yIndex)] = new InventoryCell(CellState.Empty);
                }
            }
        }
        
        public CellState GetState(Vector2Int coord)
        {
            // wersja bezpieczna: jeśli poza zakresem lub brak wpisu → traktuj jako zablokowane
            if (coord.x < 0 || coord.x >= _width || coord.y < 0 || coord.y >= _height)
                return CellState.Unreachable;

            return _cells.TryGetValue(coord, out var cell)
                ? cell.State
                : CellState.Unreachable;
        }

        public InventoryCell GetCell(Vector2Int pos)
        {
            _cells.TryGetValue(pos, out var cell);
            return cell;
        }
        
        public IEnumerable<(Vector2Int pos, InventoryCell cell)> AllCells()
        {
            // Iteracja po wszystkich legalnych polach
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var p = new Vector2Int(x, y);
                    if (_cells.TryGetValue(p, out var c))
                        yield return (p, c);
                }
            }
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