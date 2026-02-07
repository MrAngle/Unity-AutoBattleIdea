using System;
using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class InventoryGrid : IInventoryGrid {
        // TODO: make it internal
        private readonly Dictionary<Vector2Int, InventoryCell> _cells;

        internal InventoryGrid(int width, int height) {
            Width = Mathf.Max(0, width);
            Height = Mathf.Max(0, height);

            _cells = new Dictionary<Vector2Int, InventoryCell>();
            for (var xIndex = 0; xIndex < Width; xIndex++)
            for (var yIndex = 0; yIndex < Height; yIndex++)
                _cells[new Vector2Int(xIndex, yIndex)] = new InventoryCell(CellState.Empty);
        }

        public int Width { get; }

        public int Height { get; }

        public CellState getState(Vector2Int coord) {
            // wersja bezpieczna: jeśli poza zakresem lub brak wpisu → traktuj jako zablokowane
            if (coord.x < 0 || coord.x >= Width || coord.y < 0 || coord.y >= Height)
                return CellState.Unreachable;

            return _cells.TryGetValue(coord, out var cell)
                ? cell.State
                : CellState.Unreachable;
        }

        public bool canPlace(ShapeArchetype data, Vector2Int origin) {
            foreach (var off in data.Shape.Cells) {
                var p = origin + off;
                if (p.x < 0 || p.x >= Width || p.y < 0 || p.y >= Height) return false;
                if (!_cells.TryGetValue(p, out var c)) return false;
                if (!c.IsAvailableForPlacement) return false;
            }

            return true;
        }

        public void place(ShapeArchetype data, Vector2Int origin) {
            if (!canPlace(data, origin)) throw new ArgumentException("Cannot place item");

            foreach (var off in data.Shape.Cells) {
                var p = origin + off;
                _cells[p].State = CellState.Occupied;
            }
        }
    }
}