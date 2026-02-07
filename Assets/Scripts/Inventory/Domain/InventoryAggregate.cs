using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.Character.Contract;
using MageFactory.Character.Contract.Event;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Contract.Dto;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Domain {
    internal class InventoryAggregate : IGridInspector, ICharacterInventory, IInventoryInspector {
        private readonly Dictionary<Vector2Int, IInventoryPlacedItem> cellToItem = new();
        private readonly IInventoryGrid inventoryGrid;
        private readonly HashSet<IInventoryPlacedItem> items;
        private readonly IItemFactory itemFactory;
        private readonly SignalBus signalBus;

        private InventoryAggregate(IInventoryGrid inventoryGrid,
            HashSet<IInventoryPlacedEntryPoint> entryPoints,
            HashSet<IInventoryPlacedItem> items,
            IItemFactory itemFactory,
            SignalBus signalBus) {
            NullGuard.NotNullCheckOrThrow(inventoryGrid, entryPoints, items, signalBus, itemFactory);
            this.inventoryGrid = inventoryGrid;
            this.items = items;
            this.signalBus = signalBus;
            this.itemFactory = itemFactory;
            NullGuard.NotNullCheckOrThrow(this.inventoryGrid, this.items, this.itemFactory, this.signalBus);
        }

        internal static ICharacterInventory create(IItemFactory itemFactory, SignalBus signalBus) {
            IInventoryGrid
                grid = new InventoryGrid(12, 8);

            var aggregate = new InventoryAggregate(
                grid,
                new HashSet<IInventoryPlacedEntryPoint>(),
                new HashSet<IInventoryPlacedItem>(),
                itemFactory,
                signalBus
            );

            return aggregate;
        }

        public IEnumerable<ICharacterEquippedItem> getPlacedSnapshot() {
            // jeśli trzymasz IPlacedItem, dodaj na nim gettery albo mapuj z posiadanych struktur
            foreach (var item in items)
                yield return item;
        }

        public IInventoryGrid getInventoryGrid() {
            return inventoryGrid;
        }


        public bool canPlace(PlaceItemQuery placeItemQuery) {
            return inventoryGrid.canPlace(placeItemQuery.itemDefinition.getShape(), placeItemQuery.origin);
        }

        // public bool canPlace(ICharacterEquipableItem inventoryPlaceableItem, Vector2Int origin) {
        //     return inventoryGrid.canPlace(inventoryPlaceableItem.getShape(), origin);
        // }

        public ICharacterEquippedItem place(PlaceItemCommand placeItemCommand) {
            if (!inventoryGrid.canPlace(placeItemCommand.itemDefinition.getShape(), placeItemCommand.origin))
                throw new ArgumentException("Cannot place item");

            CreatePlaceableItemCommand createPlaceableItemCommand = new(placeItemCommand.itemDefinition);
            IInventoryPlaceableItem inventoryPlaceableItem = itemFactory.createPlacableItem(createPlaceableItemCommand);

            IInventoryPlacedItem inventoryPlacedItem =
                inventoryPlaceableItem.toPlacedItem(this, placeItemCommand.origin);

            if (inventoryPlacedItem.getOccupiedCells().Any(vector2Int => cellToItem.ContainsKey(vector2Int))) {
                throw new ArgumentException("Cannot place item");
            }

            items.Add(inventoryPlacedItem);
            foreach (var c in inventoryPlacedItem.getOccupiedCells()) {
                cellToItem[c] = inventoryPlacedItem;
            }

            inventoryGrid.place(inventoryPlacedItem.getShape(), placeItemCommand.origin);
            signalBus.Fire(new ItemPlacedDtoEvent(inventoryPlacedItem.getId(), inventoryPlacedItem.getShape(),
                placeItemCommand.origin));
            return inventoryPlacedItem;
        }

        // public ICharacterEquippedItem place(ICharacterEquipableItem inventoryPlaceableItem, Vector2Int origin) {
        //     if (!inventoryGrid.canPlace(inventoryPlaceableItem.getShape(), origin))
        //         throw new ArgumentException("Cannot place item");
        //
        //     itemFactory.
        //     
        //     ICharacterEquippedItem placedItem = inventoryPlaceableItem.toEquippedItem(this, origin);
        //     foreach (var c in placedItem.getOccupiedCells())
        //         if (cellToItem.ContainsKey(c))
        //             throw new ArgumentException("Cannot place item");
        //
        //     items.Add(placedItem);
        //     foreach (var c in placedItem.getOccupiedCells()) cellToItem[c] = placedItem;
        //
        //     inventoryGrid.place(placedItem.getShape(), origin);
        //     signalBus.Fire(new ItemPlacedDtoEvent(placedItem.getId(), placedItem.getShape(), origin));
        //     return placedItem;
        // }

        // public bool tryGetItemAtCell(Vector2Int cell, out ICharacterEquippedItem itemToReturn) {
        //     tryGetItemAtCell(cell, out IInventoryPlacedItem placedItem);
        //     itemToReturn = placedItem;
        //     // if (cellToItem.TryGetValue(cell, out IInventoryPlacedItem placedItem)) {
        //     //     itemToReturn = placedItem; // OK: w górę hierarchii
        //     //     return true;
        //     // }
        //     //
        //     // itemToReturn = null;
        //     // return false;
        //     // return cellToItem.TryGetValue(cell, out IInventoryPlacedItem  item);
        // }

        public bool tryGetItemAtCell(Vector2Int cell, out IInventoryPlacedItem itemToReturn) {
            if (cellToItem.TryGetValue(cell, out var placedItem)) {
                itemToReturn = placedItem;
                return true;
            }

            itemToReturn = null;
            return false;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out ICharacterEquippedItem itemToReturn) {
            var found = tryGetItemAtCell(cell, out IInventoryPlacedItem placedItem);
            itemToReturn = placedItem; // jak found == false → null
            return found;
        }
    }
}