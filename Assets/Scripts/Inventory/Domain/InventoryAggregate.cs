using System;
using System.Collections.Generic;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.Item.Controller.Domain {
    public class InventoryAggregate : IGridInspector, ICharacterInventoryFacade {
        private readonly Dictionary<Vector2Int, IPlacedItem> _cellToItem = new();

        private readonly IInventoryGrid inventoryGrid;
        private readonly HashSet<IPlacedItem> items;
        private readonly SignalBus signalBus;

        private InventoryAggregate(IInventoryGrid inventoryGrid,
            HashSet<IPlacedEntryPoint> entryPoints,
            HashSet<IPlacedItem> items,
            SignalBus signalBus) {
            NullGuard.NotNullCheckOrThrow(inventoryGrid, entryPoints, items, signalBus);
            this.inventoryGrid = inventoryGrid;
            this.items = items;
            this.signalBus = signalBus;
            NullGuard.NotNullCheckOrThrow(this.inventoryGrid, this.items, this.signalBus);
        }

        public static ICharacterInventoryFacade create(SignalBus signalBus) {
            IInventoryGrid
                grid = new InventoryGrid(12, 8);

            var aggregate = new InventoryAggregate(
                grid,
                new HashSet<IPlacedEntryPoint>(),
                new HashSet<IPlacedItem>(),
                signalBus
            );

            return aggregate;
        }

        public IEnumerable<IPlacedItem> getPlacedSnapshot() {
            // jeśli trzymasz IPlacedItem, dodaj na nim gettery albo mapuj z posiadanych struktur
            foreach (var item in items)
                yield return item;
        }

        public IInventoryGrid getInventoryGrid() {
            return inventoryGrid;
        }

        public bool canPlace(IPlaceableItem placeableItem, Vector2Int origin) {
            return inventoryGrid.canPlace(placeableItem.getShape(), origin);
        }

        public IPlacedItem place(IPlaceableItem placeableItem, Vector2Int origin) {
            if (!inventoryGrid.canPlace(placeableItem.getShape(), origin))
                throw new ArgumentException("Cannot place item");

            var placedItem = placeableItem.toPlacedItem(this, origin);
            foreach (var c in placedItem.getOccupiedCells())
                if (_cellToItem.ContainsKey(c))
                    throw new ArgumentException("Cannot place item");

            items.Add(placedItem);
            foreach (var c in placedItem.getOccupiedCells()) _cellToItem[c] = placedItem;

            inventoryGrid.place(placedItem.getShape(), origin);
            signalBus.Fire(new ItemPlacedDtoEvent(placedItem.getId(), placedItem.getShape(), origin));
            return placedItem;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out IPlacedItem item) {
            return _cellToItem.TryGetValue(cell, out item);
        }
    }
}