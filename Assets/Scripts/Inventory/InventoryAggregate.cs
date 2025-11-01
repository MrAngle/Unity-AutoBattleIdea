using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using Inventory.Slots.Domain;
using UnityEngine;

namespace Inventory {
    public class InventoryAggregate : IGridInspector {
        private readonly Dictionary<Vector2Int, IPlacedItem> _cellToItem = new();
        private readonly HashSet<IPlacedEntryPoint> _entryPoints = new();
        private readonly IInventoryGrid _inventoryGrid;
        private readonly HashSet<IPlacedItem> _items = new();

        private InventoryAggregate(IInventoryGrid inventoryGrid,
            HashSet<IPlacedEntryPoint> entryPoints,
            HashSet<IPlacedItem> items) {
            _inventoryGrid = inventoryGrid;
            _entryPoints = entryPoints;
            _items = items;
        }

        // public static InventoryAggregate Create() {
        //     IPlacedEntryPoint placedEntryPoint = PlacedEntryPoint.Create(FlowKind.Damage, new Vector2Int(0, 0), this);
        //     var inventoryGrid = IInventoryGrid.CreateInventoryGrid(8, 6, placedEntryPoint);
        //     return new InventoryAggregate(inventoryGrid, new HashSet<IPlacedEntryPoint> { placedEntryPoint }, null);
        // }
        //
        public static InventoryAggregate Create()
        {
            IInventoryGrid grid = IInventoryGrid.CreateInventoryGrid(12, 4);

            InventoryAggregate aggregate = new InventoryAggregate(
                grid,
                new HashSet<IPlacedEntryPoint>(),
                new HashSet<IPlacedItem>()
            );

            IPlacedEntryPoint placedEntryPoint = PlacedEntryPoint.Create(FlowKind.Damage, new Vector2Int(0, 0), aggregate);
            aggregate.RegisterEntryPoint(placedEntryPoint);

            return aggregate;
        }

        private void RegisterEntryPoint(IPlacedEntryPoint placedEntryPoint) {
            _entryPoints.Add(placedEntryPoint);
            _inventoryGrid.RegisterEntryPoint(placedEntryPoint);
        }


        public IInventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }

        public bool TryGetItemAtCell(Vector2Int cell, out IPlacedItem item) {
            return _cellToItem.TryGetValue(cell, out item);
        }

        private bool TryRegister(IPlacedItem item) {
            // 1) Walidacja kolizji
            foreach (var c in item.GetOccupiedCells())
                if (_cellToItem.ContainsKey(c))
                    return false;

            // 2) Commit
            _items.Add(item);
            foreach (var c in item.GetOccupiedCells())
                _cellToItem[c] = item;

            return true;
        }

        public bool Unregister(IPlacedItem item) {
            if (!_items.Remove(item)) return false;

            foreach (var c in item.GetOccupiedCells())
                _cellToItem.Remove(c);

            return true;
        }

        private static IEnumerable<Vector2Int> CellsAt(ItemData data, Vector2Int origin) {
            foreach (var off in data.Shape.Cells)
                yield return origin + off;
        }

        public bool CanPlace(ItemData data, Vector2Int origin) {
            return _inventoryGrid.CanPlace(data, origin);
        }

        public void Place(ItemData data, Vector2Int origin) {
            _inventoryGrid.Place(data, origin);
        }

        // --- MOVE (atomowo, bez alokacji zbędnych struktur) ---

        // public bool TryMove(IPlacedItem item, Vector2Int newOrigin)
        // {
        //     if (!_itemToOrigin.ContainsKey(item)) return false;
        //
        //     // Zbierz stare i nowe komórki
        //     var oldCells = item.getOccupiedCells().ToArray();
        //
        //     // Tymczasowo przestaw origin (bez commitu do słowników)
        //     var oldOrigin = item.Origin;
        //     item.SetOrigin(newOrigin);
        //     var newCells = item.getOccupiedCells().ToArray();
        //
        //     // 1) Walidacja kolizji (pozwól „nakładać się” na samego siebie)
        //     foreach (var c in newCells)
        //         if (_cellToItem.TryGetValue(c, out var other) && other != item)
        //         {
        //             // rollback origin
        //             item.SetOrigin(oldOrigin);
        //             return false;
        //         }
        //
        //     // 2) Commit: zdejmij stare wpisy, załóż nowe
        //     foreach (var c in oldCells)
        //         _cellToItem.Remove(c);
        //
        //     foreach (var c in newCells)
        //         _cellToItem[c] = item;
        //
        //     _itemToOrigin[item] = newOrigin;
        //     return true;
        // }
    }
}