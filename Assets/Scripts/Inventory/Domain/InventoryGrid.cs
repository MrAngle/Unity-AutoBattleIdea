using System;
using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class InventoryGrid : IInventoryGrid {
        private readonly Dictionary<Vector2Int, InventoryCell> _cells;
        private readonly int widthCellsNumber;
        private readonly int heightCellsNumber;
        private readonly HashSet<Vector2Int> _occupiedCells;

        internal InventoryGrid(int width, int height) {
            // Width = Mathf.Max(0, width);
            // Height = Mathf.Max(0, height);

            widthCellsNumber = Mathf.Max(0, width);
            heightCellsNumber = Mathf.Max(0, height);

            _cells = new Dictionary<Vector2Int, InventoryCell>();
            for (var xIndex = 0; xIndex < widthCellsNumber; xIndex++)
            for (var yIndex = 0; yIndex < heightCellsNumber; yIndex++)
                _cells[new Vector2Int(xIndex, yIndex)] = new InventoryCell(CellState.Empty);
        }

        // public int Width { get; }
        //
        // public int Height { get; }

        public int getHeightCellsNumber() {
            return heightCellsNumber;
        }

        public int getWidthCellsNumber() {
            return widthCellsNumber;
        }

        public CellState getState(Vector2Int coord) {
            // wersja bezpieczna: jeśli poza zakresem lub brak wpisu → traktuj jako zablokowane
            if (coord.x < 0 || coord.x >= getWidthCellsNumber() || coord.y < 0 || coord.y >= getHeightCellsNumber())
                return CellState.Unreachable;

            return _cells.TryGetValue(coord, out var cell)
                ? cell.State
                : CellState.Unreachable;
        }

        public bool canPlace(ShapeArchetype data, Vector2Int origin) {
            foreach (var cell in data.Shape.Cells) {
                Vector2Int placementCoordinate = origin + cell;
                if (placementCoordinate.x < 0 || placementCoordinate.x >= getWidthCellsNumber()
                                              || placementCoordinate.y < 0
                                              || placementCoordinate.y >= getHeightCellsNumber()) {
                    return false;
                }

                if (!_cells.TryGetValue(placementCoordinate, out var inventoryCell)) {
                    return false;
                }

                if (!inventoryCell.IsAvailableForPlacement) {
                    return false;
                }
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

        public void remove(ShapeArchetype data, Vector2Int origin) {
            foreach (var off in data.Shape.Cells) {
                var p = origin + off;
                _cells[p].State = CellState.Empty;
            }
        }
    }
}