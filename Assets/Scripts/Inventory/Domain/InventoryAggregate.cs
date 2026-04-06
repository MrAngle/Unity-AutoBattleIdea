using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.Character.Contract;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Inventory.Contract;
using MageFactory.Inventory.Contract.Dto;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.ItemSearch;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class InventoryAggregate {
        private readonly HashSet<IInventoryPlacedItem> items = new();
        private readonly Dictionary<Vector2Int, IInventoryPlacedItem> cellToItem = new();

        private readonly HashSet<IInventoryPlacedEntryPoint>
            entryPoints = new(); // to remove - use inventory related items instead

        private readonly IInventoryGrid inventoryGrid;
        private readonly IItemFactory itemFactory;

        private readonly IInventoryEventPublisher inventoryEventHub;

        private InventoryAggregate(IInventoryGrid inventoryGrid,
                                   IItemFactory itemFactory,
                                   IInventoryEventPublisher inventoryEventPublisher) {
            NullGuard.NotNullCheckOrThrow(inventoryGrid, itemFactory, inventoryEventPublisher);
            this.inventoryGrid = inventoryGrid;
            this.itemFactory = itemFactory;
            this.inventoryEventHub = inventoryEventPublisher;
            NullGuard.NotNullCheckOrThrow(this.inventoryGrid, entryPoints, items, this.itemFactory,
                this.inventoryEventHub);
        }

        internal static InventoryAggregate create(IItemFactory itemFactory,
                                                  IInventoryEventPublisher inventoryEventPublisher) {
            IInventoryGrid grid = new InventoryGrid(12, 8);

            var aggregate = new InventoryAggregate(grid, itemFactory, inventoryEventPublisher);

            return aggregate;
        }

        public IEnumerable<IGridItemPlaced> getPlacedSnapshot() {
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

        public HashSet<IInventoryPlacedEntryPoint> getEntryPointsToTick() {
            return entryPoints;
        }

        public IInventoryPlacedItem place(PlaceItemCommand placeItemCommand) {
            if (!inventoryGrid.canPlace(placeItemCommand.itemDefinition.getShape(), placeItemCommand.origin))
                throw new ArgumentException("Cannot place item");

            CreatePlaceableItemCommand createPlaceableItemCommand = new(placeItemCommand.itemDefinition);
            IInventoryPlaceableItem inventoryPlaceableItem = itemFactory.createPlacableItem(createPlaceableItemCommand);

            IInventoryPlacedItem inventoryPlacedItem =
                inventoryPlaceableItem.toPlacedItem(
                    InventoryPosition.create(placeItemCommand.origin,
                        placeItemCommand.itemDefinition.getShape().Shape));

            if (inventoryPlacedItem.getOccupiedCells().Any(vector2Int => cellToItem.ContainsKey(vector2Int))) {
                throw new ArgumentException("Cannot place item");
            }

            if (inventoryPlacedItem is IInventoryPlacedEntryPoint entryPoint) {
                entryPoints.Add(entryPoint);
            }

            items.Add(inventoryPlacedItem);
            foreach (var c in inventoryPlacedItem.getOccupiedCells()) {
                cellToItem[c] = inventoryPlacedItem;
            }

            inventoryGrid.place(inventoryPlacedItem.getShape(), placeItemCommand.origin);


            inventoryEventHub.publish(new NewItemPlacedDtoEvent(inventoryPlacedItem.getId(),
                inventoryPlacedItem.getShape(),
                placeItemCommand.origin));
            return inventoryPlacedItem;
        }

        public bool tryGetNeighborItems(IGridItemPlaced sourceGridItemPlaced,
                                        IEnumerable<GridDirection> directions,
                                        out IEnumerable<IInventoryPlacedItem> neighborItems) {
            if (sourceGridItemPlaced == null) {
                neighborItems = Enumerable.Empty<IInventoryPlacedItem>();
                return false;
            }

            IInventoryPlacedItem[] neighbors = GridAdjacencySearch
                .getNeighborItems(
                    sourceGridItemPlaced,
                    cellToItem,
                    directions)
                .ToArray();

            neighborItems = neighbors;
            return neighbors.Length > 0;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out IInventoryPlacedItem itemToReturn) {
            if (cellToItem.TryGetValue(cell, out IInventoryPlacedItem placedItem)) {
                itemToReturn = placedItem;
                return true;
            }

            itemToReturn = null;
            return false;
        }

        // public void changeItemPosition(IInventoryPlacedItem itemToMove, Vector2Int newPosition) {
        //     InventoryPosition inventoryPosition = InventoryPosition.create(newPosition, itemToMove.getShape().Shape);
        //     itemToMove.updateItemPosition(inventoryPosition);
        // }

        // it must be thread safe / transactional
        public void changeItemPosition(Id<ItemId> idOfItemToMove, Vector2Int newPosition) {
            IInventoryPlacedItem itemToMove = items
                .FirstOrDefault(item => item.getId().Equals(idOfItemToMove));

            if (itemToMove == null) {
                Debug.LogError("Item does not belong to this inventory. ItemId:" + idOfItemToMove);
                throw new ArgumentException("Item does not belong to this inventory. ItemId:" + idOfItemToMove);
            }

            var shape = itemToMove.getShape();
            Vector2Int oldItemOrigin = itemToMove.getOrigin();
            var oldOccupiedCells = itemToMove.getOccupiedCells().ToArray();

            InventoryPosition newInventoryPosition = InventoryPosition.create(newPosition, shape.Shape);
            var newOccupiedCells = newInventoryPosition.getOccupiedCells().ToArray();

            foreach (var oldCell in oldOccupiedCells) {
                if (cellToItem.TryGetValue(oldCell, out var mappedItem) && ReferenceEquals(mappedItem, itemToMove)) {
                    cellToItem.Remove(oldCell);
                }
            }

            inventoryGrid.remove(itemToMove.getShape(), itemToMove.getOrigin());

            if (!inventoryGrid.canPlace(shape, newPosition)) {
                Debug.LogError("Cannot move item to the target position. " + nameof(itemToMove));
                throw new ArgumentException("Cannot move item to the target position.", nameof(itemToMove));
            }

            foreach (var cell in newOccupiedCells) {
                if (cellToItem.TryGetValue(cell, out var occupyingItem) &&
                    !ReferenceEquals(occupyingItem, itemToMove)) {
                    Debug.LogError(
                        "Cannot move item - target cells are occupied by another item. " + nameof(itemToMove));
                    throw new ArgumentException(
                        "Cannot move item - target cells are occupied by another item.",
                        nameof(newPosition)
                    );
                }
            }

            // foreach (var oldCell in oldOccupiedCells) {
            //     if (cellToItem.TryGetValue(oldCell, out var mappedItem) && ReferenceEquals(mappedItem, itemToMove)) {
            //         cellToItem.Remove(oldCell);
            //     }
            // }

            itemToMove.updateItemPosition(newInventoryPosition);

            foreach (var newCell in newOccupiedCells) {
                cellToItem[newCell] = itemToMove;
            }

            inventoryEventHub.publish(new ItemPositionChangedDtoEvent(itemToMove.getId(),
                itemToMove.getShape(),
                newPosition,
                oldItemOrigin));
        }
    }
}