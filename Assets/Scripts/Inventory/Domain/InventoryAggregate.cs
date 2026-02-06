using System;
using System.Collections.Generic;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Domain {
    // TODO: rozdzielic characterInventoryFacade od inventoryAggregate
    // public interface ICharacterInventoryFacade {
    //     IEnumerable<IPlacedItem> GetPlacedSnapshot();
    //     IInventoryGrid GetInventoryGrid();
    //     public IPlacedItem Place(IPlaceableItem placeableItem, Vector2Int origin);
    //     public bool CanPlace(IPlaceableItem placeableItem, Vector2Int origin);
    // }
    //
    public class InventoryAggregate : IGridInspector, ICharacterInventoryFacade {
        private readonly Dictionary<Vector2Int, IPlacedItem> _cellToItem = new();

        private readonly IEntryPointFactory _entryPointFactory;

        // private readonly HashSet<IPlacedEntryPoint> _entryPoints;
        private readonly IInventoryGrid _inventoryGrid;
        private readonly HashSet<IPlacedItem> _items;
        private readonly SignalBus _signalBus;

        private InventoryAggregate(IInventoryGrid inventoryGrid,
            HashSet<IPlacedEntryPoint> entryPoints,
            HashSet<IPlacedItem> items,
            SignalBus signalBus,
            IEntryPointFactory entryPointFactory) {
            NullGuard.NotNullCheckOrThrow(inventoryGrid, entryPoints, items, signalBus, entryPointFactory);
            _inventoryGrid = inventoryGrid;
            // _entryPoints = entryPoints;
            _items = items;
            _signalBus = signalBus;
            _entryPointFactory = entryPointFactory;
            NullGuard.NotNullCheckOrThrow(_inventoryGrid, _items, _signalBus, _entryPointFactory);
        }

        public IEnumerable<IPlacedItem> GetPlacedSnapshot() {
            // jeśli trzymasz IPlacedItem, dodaj na nim gettery albo mapuj z posiadanych struktur
            foreach (var item in _items)
                yield return item;
        }

        public IInventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }

        public bool CanPlace(IPlaceableItem placeableItem, Vector2Int origin) {
            return _inventoryGrid.CanPlace(placeableItem.GetShape(), origin);
        }

        public IPlacedItem Place(IPlaceableItem placeableItem, Vector2Int origin) {
            if (!_inventoryGrid.CanPlace(placeableItem.GetShape(), origin))
                throw new ArgumentException("Cannot place item");

            var placedItem = placeableItem.ToPlacedItem(this, origin);
            foreach (var c in placedItem.GetOccupiedCells())
                if (_cellToItem.ContainsKey(c))
                    throw new ArgumentException("Cannot place item");

            _items.Add(placedItem);
            foreach (var c in placedItem.GetOccupiedCells()) _cellToItem[c] = placedItem;

            _inventoryGrid.Place(placedItem.GetShape(), origin);
            _signalBus.Fire(new ItemPlacedDtoEvent(placedItem.GetId(), placedItem.GetShape(), origin));
            return placedItem;
        }

        public bool TryGetItemAtCell(Vector2Int cell, out IPlacedItem item) {
            return _cellToItem.TryGetValue(cell, out item);
        }

        public static ICharacterInventoryFacade Create(SignalBus signalBus, IEntryPointFactory entryPointFactory) {
            IInventoryGrid
                grid = new InventoryGrid(12, 8); // previously used IInventoryGrid.CreateInventoryGrid(12, 8);

            var aggregate = new InventoryAggregate(
                grid,
                new HashSet<IPlacedEntryPoint>(),
                new HashSet<IPlacedItem>(),
                signalBus,
                entryPointFactory
            );

            return aggregate;
        }
    }
}