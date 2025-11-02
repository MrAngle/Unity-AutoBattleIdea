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

        public void Place(GridEntryPoint entryPoint, Vector2Int origin) {
            IPlacedEntryPoint placedEntryPoint = _entryPointFactory.CreatePlacedEntryPoint(entryPoint.GetFlowKind(), origin, this);

            _entryPoints.Add(placedEntryPoint);
            _inventoryGrid.RegisterEntryPoint(placedEntryPoint);
            
            // TODO
            // _signalBus.Fire(new ItemPlacedDtoEvent(placedEntryPoint.GetId(), data, origin));
        }

        public IInventoryGrid GetInventoryGrid() {
            return _inventoryGrid;
        }

        public bool TryGetItemAtCell(Vector2Int cell, out IPlacedItem item) {
            return _cellToItem.TryGetValue(cell, out item);
        }

        public bool CanPlace(ItemData data, Vector2Int origin) {
            return _inventoryGrid.CanPlace(data, origin);
        }

        public IPlacedItem Place(ItemData data, Vector2Int origin) {
            if (!_inventoryGrid.CanPlace(data, origin)) {
                throw new System.ArgumentException("Cannot place item");
            }

            IPlacedItem item = IPlacedItem.CreateBattleItem(data, origin);
            foreach (var c in item.GetOccupiedCells()) {
                if (_cellToItem.ContainsKey(c)) {
                    throw new System.ArgumentException("Cannot place item");
                }
            }

            _items.Add(item);
            foreach (var c in item.GetOccupiedCells()) {
                _cellToItem[c] = item;
            }

            _inventoryGrid.Place(data, origin);
            _signalBus.Fire(new ItemPlacedDtoEvent(item.GetId(), data, origin));
            return item;
        }
    }
}