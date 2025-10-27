using System.Collections.Generic;
using System.Collections.ObjectModel;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using UnityEngine;

namespace Inventory.Slots.Domain {
    
    public interface IInventoryGrid
    {
        int Width { get; }
        int Height { get; }
        CellState GetState(Vector2Int coord);

        bool CanPlace(ItemData data, Vector2Int origin);
        void Place(ItemData data, Vector2Int origin);
        
        public static IInventoryGrid CreateInventoryGrid(int width, int height, IEntryPointFacade entryPoint) {
            return new InventoryGrid(width, height, entryPoint);
        }
    }

    
    class InventoryGrid : IInventoryGrid {
        private readonly Dictionary<Vector2Int, InventoryCell> _cells;
        private readonly int _width;
        private readonly int _height;
        
        private readonly List<IEntryPointFacade> _entryPoints = new();
        public ReadOnlyCollection<IEntryPointFacade> EntryPoints => _entryPoints.AsReadOnly();
        
        public int Width => _width;
        public int Height => _height;

        internal InventoryGrid(int width, int height, IEntryPointFacade entryPoint)
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
            
            TryAddEntryPoint(entryPoint);
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
        
        public bool CanPlace(ItemData data, Vector2Int origin)
        {
            foreach (var off in data.Shape.Cells)
            {
                var p = origin + off;
                if (p.x < 0 || p.x >= _width || p.y < 0 || p.y >= _height) return false;
                if (!_cells.TryGetValue(p, out var c)) return false;
                if (!c.IsAvailableForPlacement) return false;
            }
            return true;
        }

        public void Place(ItemData data, Vector2Int origin)
        {
            if (!CanPlace(data, origin)) {
                throw new System.ArgumentException("Cannot place item");
            }

            foreach (var off in data.Shape.Cells)
            {
                var p = origin + off;
                _cells[p].State = CellState.Occupied;
            }
        }

        public void Remove(ItemData data, Vector2Int origin)
        {
            foreach (var off in data.Shape.Cells)
            {
                var p = origin + off;
                if (_cells.TryGetValue(p, out var c)) c.State = CellState.Empty;
            }
        }
        
        private bool IsWithinBounds(Vector2Int p)
            => p.x >= 0 && p.x < _width && p.y >= 0 && p.y < _height;

        /// Dodaj punkt wejścia (np. Damage) pod warunkiem, że mieści się w siatce.
        public void TryAddEntryPoint(IEntryPointFacade entry)
        {
            if (!IsWithinBounds(entry.GetPosition())) throw new System.ArgumentException("Entry point is out of bounds");
            _entryPoints.Add(entry);
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