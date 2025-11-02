using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using Inventory.Items.View;
using Inventory.Slots.Domain;
using Shared.Utility;
using UnityEngine;
using Zenject;

namespace Inventory {
    public class InventoryAggregate : IGridInspector {
        private readonly SignalBus _signalBus;
        private readonly HashSet<IPlacedEntryPoint> _entryPoints;
        private readonly IInventoryGrid _inventoryGrid;
        private readonly HashSet<IPlacedItem> _items;
        private readonly IEntryPointFactory _entryPointFactory;
        
        private readonly Dictionary<Vector2Int, IPlacedItem> _cellToItem = new();

        private InventoryAggregate(IInventoryGrid inventoryGrid,
            HashSet<IPlacedEntryPoint> entryPoints,
            HashSet<IPlacedItem> items,
            SignalBus signalBus,
            IEntryPointFactory entryPointFactory) {
            NullGuard.NotNullCheckOrThrow(inventoryGrid, entryPoints, items, signalBus, entryPointFactory);
            _inventoryGrid = inventoryGrid;
            _entryPoints = entryPoints;
            _items = items;
            _signalBus = signalBus;
            _entryPointFactory = entryPointFactory;
            NullGuard.NotNullCheckOrThrow(_inventoryGrid, _entryPoints, _items, _signalBus, _entryPointFactory);
        }

        public static InventoryAggregate Create(SignalBus signalBus, IEntryPointFactory entryPointFactory)
        {
            IInventoryGrid grid = IInventoryGrid.CreateInventoryGrid(12, 8);

            InventoryAggregate aggregate = new InventoryAggregate(
                grid,
                new HashSet<IPlacedEntryPoint>(),
                new HashSet<IPlacedItem>(),
                signalBus,
                entryPointFactory
            );

            return aggregate;
        }

        public IInventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }

        public bool TryGetItemAtCell(Vector2Int cell, out IPlacedItem item) {
            return _cellToItem.TryGetValue(cell, out item);
        }

        public bool CanPlace(IPlaceableItem placeableItem, Vector2Int origin) {
            return _inventoryGrid.CanPlace(placeableItem.GetShape(), origin);
        }

        public IPlacedItem Place(IPlaceableItem placeableItem, Vector2Int origin) {
            if (!_inventoryGrid.CanPlace(placeableItem.GetShape(), origin)) {
                throw new System.ArgumentException("Cannot place item");
            }
            
            IPlacedItem placedItem = placeableItem.ToPlacedItem(this, origin);
            foreach (var c in placedItem.GetOccupiedCells()) {
                if (_cellToItem.ContainsKey(c)) {
                    throw new System.ArgumentException("Cannot place item");
                }
            }

            _items.Add(placedItem);
            foreach (var c in placedItem.GetOccupiedCells()) {
                _cellToItem[c] = placedItem;
            }

            _inventoryGrid.Place(placedItem.GetShape(), origin);
            _signalBus.Fire(new ItemPlacedDtoEvent(placedItem.GetId(), placedItem.GetShape(), origin));
            return placedItem;
        }
    }
}