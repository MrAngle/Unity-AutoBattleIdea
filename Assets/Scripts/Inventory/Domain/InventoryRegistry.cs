using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.ItemSearch;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class InventoryRegistry {
        // it should never be published by getter etc. It must be only 1 source of truth otherwise this class will be a mess
        private readonly HashSet<IInventoryPlacedItem> items;
        private readonly Dictionary<Vector2Int, InventoryCell> cells;
        private readonly int widthCellsNumber;
        private readonly int heightCellsNumber;

        // Indexes - it may be better to group them.
        private readonly Dictionary<Vector2Int, IInventoryPlacedItem> cellToItem;
        private readonly Dictionary<Id<ItemId>, IInventoryPlacedItem> itemIdToItem;

        private InventoryRegistry(
            int widthCellsNumber,
            int heightCellsNumber,
            HashSet<IInventoryPlacedItem> items,
            Dictionary<Vector2Int, IInventoryPlacedItem> cellToItem,
            Dictionary<Vector2Int, InventoryCell> cells,
            Dictionary<Id<ItemId>, IInventoryPlacedItem> itemIdToItem) {
            if (widthCellsNumber < 1) {
                throw new ArgumentOutOfRangeException(nameof(widthCellsNumber), "Width must be greater than 0.");
            }

            if (heightCellsNumber < 1) {
                throw new ArgumentOutOfRangeException(nameof(heightCellsNumber), "Height must be greater than 0.");
            }

            this.widthCellsNumber = widthCellsNumber;
            this.heightCellsNumber = heightCellsNumber;
            this.items = NullGuard.NotNullOrThrow(items);
            this.cellToItem = NullGuard.NotNullOrThrow(cellToItem);
            this.cells = NullGuard.NotNullOrThrow(cells);
            this.itemIdToItem = NullGuard.NotNullOrThrow(itemIdToItem);
        }

        public static InventoryRegistry createNew(int widthCellsNumber, int heightCellsNumber) {
            Dictionary<Vector2Int, InventoryCell> cells = new();

            for (int xIndex = 0; xIndex < widthCellsNumber; xIndex++) {
                for (int yIndex = 0; yIndex < heightCellsNumber; yIndex++) {
                    cells[new Vector2Int(xIndex, yIndex)] = new InventoryCell(CellState.Empty);
                }
            }

            return new InventoryRegistry(
                widthCellsNumber,
                heightCellsNumber,
                new HashSet<IInventoryPlacedItem>(),
                new Dictionary<Vector2Int, IInventoryPlacedItem>(),
                cells,
                new Dictionary<Id<ItemId>, IInventoryPlacedItem>()
            );
        }

        public int getHeightCellsNumber() {
            return heightCellsNumber;
        }

        public int getWidthCellsNumber() {
            return widthCellsNumber;
        }

        public IEnumerable<IGridItemPlaced> getPlacedSnapshot() {
            foreach (var item in items)
                yield return item;
        }

        internal CellState getCellState(Vector2Int coord) {
            if (coord.x < 0 || coord.x >= getWidthCellsNumber() || coord.y < 0 || coord.y >= getHeightCellsNumber())
                return CellState.Unreachable;

            return cells.TryGetValue(coord, out var cell)
                ? cell.State
                : CellState.Unreachable;
        }

        internal bool isCellNotAvailableForPlacement(Vector2Int cell) {
            return !isCellAvailableForPlacement(cell);
        }

        internal bool isCellAvailableForPlacement(Vector2Int cell) {
            if (!cells.TryGetValue(cell, out var value) || value == null) {
                return false;
            }

            if (!value.IsAvailableForPlacement) {
                return false;
            }

            return !cellToItem.ContainsKey(cell);
        }

        internal bool canPlaceItem(ShapeArchetype shape, Vector2Int origin) {
            foreach (var cellOffset in shape.Shape.Cells) {
                Vector2Int targetCell = origin + cellOffset;

                if (!cells.TryGetValue(targetCell, out var inventoryCell) || inventoryCell == null) {
                    return false;
                }

                if (!inventoryCell.IsAvailableForPlacement) {
                    return false;
                }

                if (cellToItem.ContainsKey(targetCell)) {
                    return false;
                }
            }

            return true;
        }

        internal void placeItem(IInventoryPlacedItem inventoryPlacedItem) {
            NullGuard.NotNullOrThrow(inventoryPlacedItem);

            if (items.Contains(inventoryPlacedItem)) {
                throw new InvalidOperationException("Item is already registered in inventory.");
            }

            if (itemIdToItem.ContainsKey(inventoryPlacedItem.getId())) {
                throw new InvalidOperationException($"Item id {inventoryPlacedItem.getId()} is already registered.");
            }

            foreach (var occupiedCell in inventoryPlacedItem.getOccupiedCells()) {
                if (!cells.TryGetValue(occupiedCell, out var inventoryCell) || inventoryCell == null) {
                    throw new KeyNotFoundException($"Cell {occupiedCell} does not exist in inventory.");
                }

                if (!inventoryCell.IsAvailableForPlacement) {
                    throw new InvalidOperationException($"Cell {occupiedCell} is not available for placement.");
                }

                if (cellToItem.ContainsKey(occupiedCell)) {
                    throw new InvalidOperationException($"Cell {occupiedCell} is already occupied.");
                }
            }

            items.Add(inventoryPlacedItem);
            itemIdToItem[inventoryPlacedItem.getId()] = inventoryPlacedItem;

            foreach (var occupiedCell in inventoryPlacedItem.getOccupiedCells()) {
                cellToItem[occupiedCell] = inventoryPlacedItem;
                cells[occupiedCell].State = CellState.Occupied;
            }
        }

        internal void removeItem(Id<ItemId> itemId) {
            var inventoryPlacedItem = getItemOrThrow(itemId);

            foreach (var occupiedCell in inventoryPlacedItem.getOccupiedCells()) {
                if (cellToItem.TryGetValue(occupiedCell, out var mappedItem)
                    && ReferenceEquals(mappedItem, inventoryPlacedItem)) {
                    cellToItem.Remove(occupiedCell);
                }

                if (cells.TryGetValue(occupiedCell, out var inventoryCell) && inventoryCell != null) {
                    inventoryCell.State = CellState.Empty;
                }
            }

            items.Remove(inventoryPlacedItem);
            itemIdToItem.Remove(itemId);
        }

        internal bool tryGetItemAtCell(Vector2Int cell, out IInventoryPlacedItem itemToReturn) {
            if (cellToItem.TryGetValue(cell, out var placedItem)) {
                itemToReturn = placedItem;
                return true;
            }

            itemToReturn = null;
            return false;
        }

        internal IEnumerable<IInventoryPlacedItem> getNeighborItems(IGridItemPlaced sourceGridItemPlaced,
                                                                    IEnumerable<GridDirection> directions) {
            if (sourceGridItemPlaced == null) {
                return Enumerable.Empty<IInventoryPlacedItem>();
            }

            var neighbors = GridAdjacencySearch
                .getNeighborItems(
                    sourceGridItemPlaced,
                    cellToItem,
                    directions)
                .ToArray();

            return neighbors;
        }

        internal bool tryMoveItem(Id<ItemId> idOfItemToMove,
                                  Vector2Int newOriginCellPosition,
                                  out IInventoryPlacedItem movedItem,
                                  out Vector2Int oldOriginCellPosition) {
            movedItem = null;
            oldOriginCellPosition = default;

            if (!itemIdToItem.TryGetValue(idOfItemToMove, out var inventoryPlacedItem)) {
                return false;
            }

            var shape = inventoryPlacedItem.getShape();
            oldOriginCellPosition = inventoryPlacedItem.getOrigin();
            var oldOccupiedCells = inventoryPlacedItem.getOccupiedCells().ToArray();

            var newInventoryPosition = InventoryPosition.create(newOriginCellPosition, shape.Shape);
            var newOccupiedCells = newInventoryPosition.getOccupiedCells().ToArray();

            if (!canMoveItemToTarget(inventoryPlacedItem, newOccupiedCells)) {
                return false;
            }

            try {
                clearOccupiedCellsForItem(inventoryPlacedItem, oldOccupiedCells);
                updateItemPositionAndRegisterNewCells(inventoryPlacedItem, newInventoryPosition, newOccupiedCells);
                movedItem = inventoryPlacedItem;
                return true;
            }
            catch {
                restoreItemPositionAndOccupiedCells(
                    inventoryPlacedItem,
                    oldOriginCellPosition,
                    shape,
                    oldOccupiedCells
                );
                throw;
            }
        }

        private bool canMoveItemToTarget(IInventoryPlacedItem inventoryPlacedItem,
                                         IEnumerable<Vector2Int> newOccupiedCells) {
            foreach (var newCell in newOccupiedCells) {
                if (!cells.TryGetValue(newCell, out var inventoryCell) || inventoryCell == null) {
                    return false;
                }

                if (cellToItem.TryGetValue(newCell, out var occupyingItem)
                    && !ReferenceEquals(occupyingItem, inventoryPlacedItem)) {
                    return false;
                }
            }

            return true;
        }

        private IInventoryPlacedItem getItemOrThrow(Id<ItemId> itemId) {
            if (itemIdToItem.TryGetValue(itemId, out var inventoryPlacedItem)) {
                return inventoryPlacedItem;
            }

            throw new KeyNotFoundException($"Item is not registered in inventory. ItemId: {itemId}");
        }

        private void clearOccupiedCellsForItem(
            IInventoryPlacedItem inventoryPlacedItem,
            IEnumerable<Vector2Int> occupiedCells) {
            foreach (var occupiedCell in occupiedCells) {
                if (cellToItem.TryGetValue(occupiedCell, out var mappedItem)
                    && ReferenceEquals(mappedItem, inventoryPlacedItem)) {
                    cellToItem.Remove(occupiedCell);
                }

                if (cells.TryGetValue(occupiedCell, out var inventoryCell) && inventoryCell != null) {
                    inventoryCell.State = CellState.Empty;
                }
            }
        }

        private void updateItemPositionAndRegisterNewCells(
            IInventoryPlacedItem inventoryPlacedItem,
            InventoryPosition newInventoryPosition,
            IEnumerable<Vector2Int> newOccupiedCells) {
            inventoryPlacedItem.updateItemPosition(newInventoryPosition);

            foreach (var newCell in newOccupiedCells) {
                cellToItem[newCell] = inventoryPlacedItem;
                cells[newCell].State = CellState.Occupied;
            }
        }

        private void restoreItemPositionAndOccupiedCells(
            IInventoryPlacedItem inventoryPlacedItem,
            Vector2Int oldOriginCellPosition,
            ShapeArchetype shape,
            IEnumerable<Vector2Int> oldOccupiedCells) {
            var oldInventoryPosition = InventoryPosition.create(oldOriginCellPosition, shape.Shape);
            inventoryPlacedItem.updateItemPosition(oldInventoryPosition);

            foreach (var oldCell in oldOccupiedCells) {
                cellToItem[oldCell] = inventoryPlacedItem;

                if (cells.TryGetValue(oldCell, out var inventoryCell) && inventoryCell != null) {
                    inventoryCell.State = CellState.Occupied;
                }
            }
        }
    }
}