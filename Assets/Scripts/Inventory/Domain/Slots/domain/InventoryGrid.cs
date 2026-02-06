using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MageFactory.Inventory.Api;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    public class InventoryGrid : IInventoryGrid {
        // TODO: make it internal
        private readonly Dictionary<Vector2Int, InventoryCell> _cells;

        private readonly List<IPlacedEntryPoint> _entryPoints = new();

        internal InventoryGrid(int width, int height, IPlacedEntryPoint placedEntryPoint = null) {
            Width = Mathf.Max(0, width);
            Height = Mathf.Max(0, height);

            _cells = new Dictionary<Vector2Int, InventoryCell>();
            for (var xIndex = 0; xIndex < Width; xIndex++)
            for (var yIndex = 0; yIndex < Height; yIndex++)
                _cells[new Vector2Int(xIndex, yIndex)] = new InventoryCell(CellState.Empty);

            if (placedEntryPoint != null) TryAddEntryPoint(placedEntryPoint);
        }

        public ReadOnlyCollection<IPlacedEntryPoint> EntryPoints => _entryPoints.AsReadOnly();

        public int Width { get; }

        public int Height { get; }

        public CellState GetState(Vector2Int coord) {
            // wersja bezpieczna: jeśli poza zakresem lub brak wpisu → traktuj jako zablokowane
            if (coord.x < 0 || coord.x >= Width || coord.y < 0 || coord.y >= Height)
                return CellState.Unreachable;

            return _cells.TryGetValue(coord, out var cell)
                ? cell.State
                : CellState.Unreachable;
        }

        public InventoryCell GetCell(Vector2Int pos) {
            _cells.TryGetValue(pos, out var cell);
            return cell;
        }

        public IEnumerable<(Vector2Int pos, InventoryCell cell)> AllCells() {
            // Iteracja po wszystkich legalnych polach
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++) {
                var p = new Vector2Int(x, y);
                if (_cells.TryGetValue(p, out var c))
                    yield return (p, c);
            }
        }

        public bool CanPlace(ShapeArchetype data, Vector2Int origin) {
            foreach (var off in data.Shape.Cells) {
                var p = origin + off;
                if (p.x < 0 || p.x >= Width || p.y < 0 || p.y >= Height) return false;
                if (!_cells.TryGetValue(p, out var c)) return false;
                if (!c.IsAvailableForPlacement) return false;
            }

            return true;
        }

        public void Place(ShapeArchetype data, Vector2Int origin) {
            if (!CanPlace(data, origin)) throw new ArgumentException("Cannot place item");

            foreach (var off in data.Shape.Cells) {
                var p = origin + off;
                _cells[p].State = CellState.Occupied;
            }
        }

        public void RegisterEntryPoint(IPlacedEntryPoint placedEntryPoint) {
            TryAddEntryPoint(placedEntryPoint);
            // throw new System.NotImplementedException();
        }

        public void Remove(ShapeArchetype data, Vector2Int origin) {
            foreach (var off in data.Shape.Cells) {
                var p = origin + off;
                if (_cells.TryGetValue(p, out var c)) c.State = CellState.Empty;
            }
        }

        private bool IsWithinBounds(Vector2Int p) {
            return p.x >= 0 && p.x < Width && p.y >= 0 && p.y < Height;
        }

        /// Dodaj punkt wejścia (np. Damage) pod warunkiem, że mieści się w siatce.
        public void TryAddEntryPoint(IPlacedEntryPoint placedEntry) {
            if (!IsWithinBounds(placedEntry.GetOccupiedCells().First() /*for now*/))
                throw new ArgumentException("Entry point is out of bounds");
            _entryPoints.Add(placedEntry);
        }

        // Usuń po Id (zwraca true, jeśli coś usunięto)
        // public bool RemoveEntryPoint(string id)
        // {
        //     var idx = _entryPoints.FindIndex(e => e.Id == id);
        //     if (idx < 0) return false;
        //     _entryPoints.RemoveAt(idx);
        //     return true;
        // }

        // Zwraca wszystkie wejścia danego typu (Damage/Defense).
        // public IEnumerable<GridEntryPoint> GetEntryPoints(FlowKind kind)
        //     => _entryPoints.Where(e => e.Kind == kind);

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