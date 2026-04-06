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
        private readonly HashSet<IInventoryPlacedItem> items;
        private readonly Dictionary<Vector2Int, InventoryCell> cells;
        private readonly int widthCellsNumber;
        private readonly int heightCellsNumber;
        private readonly InventoryRegistryIndexes indexes;

        private InventoryRegistry(
            int widthCellsNumber,
            int heightCellsNumber,
            HashSet<IInventoryPlacedItem> items,
            Dictionary<Vector2Int, InventoryCell> cells) {
            if (widthCellsNumber < 1) {
                throw new ArgumentOutOfRangeException(nameof(widthCellsNumber), "Width must be greater than 0.");
            }

            if (heightCellsNumber < 1) {
                throw new ArgumentOutOfRangeException(nameof(heightCellsNumber), "Height must be greater than 0.");
            }

            this.widthCellsNumber = widthCellsNumber;
            this.heightCellsNumber = heightCellsNumber;
            this.items = NullGuard.NotNullOrThrow(items);
            this.cells = NullGuard.NotNullOrThrow(cells);

            indexes = new InventoryRegistryIndexes();
            rebuildIndexes();
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
                cells
            );
        }

        public int getHeightCellsNumber() {
            return heightCellsNumber;
        }

        public int getWidthCellsNumber() {
            return widthCellsNumber;
        }

        public IReadOnlyCollection<IInventoryPlacedEntryPoint> getEntryPoints() {
            return indexes.getEntryPoints();
        }

        public IEnumerable<IGridItemPlaced> getPlacedSnapshot() {
            foreach (IInventoryPlacedItem item in items) {
                yield return item;
            }
        }

        internal CellState getCellState(Vector2Int coord) {
            if (coord.x < 0 || coord.x >= getWidthCellsNumber() || coord.y < 0 || coord.y >= getHeightCellsNumber()) {
                return CellState.Unreachable;
            }

            return cells.TryGetValue(coord, out InventoryCell cell)
                ? cell.State
                : CellState.Unreachable;
        }

        internal bool isCellNotAvailableForPlacement(Vector2Int cell) {
            return !isCellAvailableForPlacement(cell);
        }

        internal bool isCellAvailableForPlacement(Vector2Int cell) {
            if (!cells.TryGetValue(cell, out InventoryCell value) || value == null) {
                return false;
            }

            if (!value.IsAvailableForPlacement) {
                return false;
            }

            return !indexes.isCellOccupied(cell);
        }

        internal bool canPlaceItem(ShapeArchetype shape, Vector2Int origin) {
            foreach (Vector2Int cellOffset in shape.Shape.Cells) {
                Vector2Int targetCell = origin + cellOffset;

                if (!cells.TryGetValue(targetCell, out InventoryCell inventoryCell) || inventoryCell == null) {
                    return false;
                }

                if (!inventoryCell.IsAvailableForPlacement) {
                    return false;
                }

                if (indexes.isCellOccupied(targetCell)) {
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

            if (indexes.tryGetItem(inventoryPlacedItem.getId(), out _)) {
                throw new InvalidOperationException($"Item id {inventoryPlacedItem.getId()} is already registered.");
            }

            if (!canPlaceItem(inventoryPlacedItem.getShape(), inventoryPlacedItem.getOrigin())) {
                throw new InvalidOperationException("Item cannot be placed in the target position.");
            }

            items.Add(inventoryPlacedItem);
            indexes.addItem(inventoryPlacedItem);

            foreach (Vector2Int occupiedCell in inventoryPlacedItem.getOccupiedCells()) {
                cells[occupiedCell].State = CellState.Occupied;
            }
        }

        internal void removeItem(Id<ItemId> itemId) {
            IInventoryPlacedItem inventoryPlacedItem = getItemOrThrow(itemId);

            foreach (Vector2Int occupiedCell in inventoryPlacedItem.getOccupiedCells()) {
                if (cells.TryGetValue(occupiedCell, out InventoryCell inventoryCell) && inventoryCell != null) {
                    inventoryCell.State = CellState.Empty;
                }
            }

            items.Remove(inventoryPlacedItem);
            indexes.removeItem(inventoryPlacedItem);
        }

        internal bool tryGetItemAtCell(Vector2Int cell, out IInventoryPlacedItem itemToReturn) {
            return indexes.tryGetItemAtCell(cell, out itemToReturn);
        }

        internal IEnumerable<IInventoryPlacedItem> getNeighborItems(
            IGridItemPlaced sourceGridItemPlaced,
            IEnumerable<GridDirection> directions) {
            if (sourceGridItemPlaced == null) {
                return Enumerable.Empty<IInventoryPlacedItem>();
            }

            IInventoryPlacedItem[] neighbors = GridAdjacencySearch
                .getNeighborItems(
                    sourceGridItemPlaced,
                    indexes.getCellToItem(),
                    directions)
                .ToArray();

            return neighbors;
        }

        internal bool tryMoveItem(
            Id<ItemId> idOfItemToMove,
            Vector2Int newOriginCellPosition,
            out IInventoryPlacedItem movedItem,
            out Vector2Int oldOriginCellPosition) {
            movedItem = null;
            oldOriginCellPosition = default;

            if (!indexes.tryGetItem(idOfItemToMove, out IInventoryPlacedItem inventoryPlacedItem)) {
                return false;
            }

            ShapeArchetype shape = inventoryPlacedItem.getShape();
            oldOriginCellPosition = inventoryPlacedItem.getOrigin();
            Vector2Int[] oldOccupiedCells = inventoryPlacedItem.getOccupiedCells().ToArray();

            InventoryPosition newInventoryPosition = InventoryPosition.create(newOriginCellPosition, shape.Shape);
            Vector2Int[] newOccupiedCells = newInventoryPosition.getOccupiedCells().ToArray();

            if (!canMoveItemToTarget(inventoryPlacedItem, newOccupiedCells)) {
                return false;
            }

            try {
                clearOccupiedCellsForItem(inventoryPlacedItem, oldOccupiedCells);
                inventoryPlacedItem.updateItemPosition(newInventoryPosition);
                indexes.updateItem(inventoryPlacedItem, oldOccupiedCells);

                foreach (Vector2Int newCell in newOccupiedCells) {
                    cells[newCell].State = CellState.Occupied;
                }

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

        private bool canMoveItemToTarget(
            IInventoryPlacedItem inventoryPlacedItem,
            IEnumerable<Vector2Int> newOccupiedCells) {
            foreach (Vector2Int newCell in newOccupiedCells) {
                if (!cells.TryGetValue(newCell, out InventoryCell inventoryCell) || inventoryCell == null) {
                    return false;
                }

                if (!inventoryCell.IsAvailableForPlacement) {
                    return false;
                }

                if (indexes.tryGetItemAtCell(newCell, out IInventoryPlacedItem occupyingItem)
                    && !ReferenceEquals(occupyingItem, inventoryPlacedItem)) {
                    return false;
                }
            }

            return true;
        }

        private IInventoryPlacedItem getItemOrThrow(Id<ItemId> itemId) {
            if (indexes.tryGetItem(itemId, out IInventoryPlacedItem inventoryPlacedItem)) {
                return inventoryPlacedItem;
            }

            throw new KeyNotFoundException($"Item is not registered in inventory. ItemId: {itemId}");
        }

        private void clearOccupiedCellsForItem(
            IInventoryPlacedItem inventoryPlacedItem,
            IEnumerable<Vector2Int> occupiedCells) {
            foreach (Vector2Int occupiedCell in occupiedCells) {
                if (cells.TryGetValue(occupiedCell, out InventoryCell inventoryCell) && inventoryCell != null) {
                    inventoryCell.State = CellState.Empty;
                }
            }
        }

        private void restoreItemPositionAndOccupiedCells(
            IInventoryPlacedItem inventoryPlacedItem,
            Vector2Int oldOriginCellPosition,
            ShapeArchetype shape,
            IEnumerable<Vector2Int> oldOccupiedCells) {
            InventoryPosition oldInventoryPosition = InventoryPosition.create(oldOriginCellPosition, shape.Shape);
            inventoryPlacedItem.updateItemPosition(oldInventoryPosition);

            indexes.updateItem(inventoryPlacedItem, inventoryPlacedItem.getOccupiedCells());

            foreach (Vector2Int oldCell in oldOccupiedCells) {
                if (cells.TryGetValue(oldCell, out InventoryCell inventoryCell) && inventoryCell != null) {
                    inventoryCell.State = CellState.Occupied;
                }
            }
        }

        private void rebuildIndexes() {
            indexes.clear();

            foreach (KeyValuePair<Vector2Int, InventoryCell> pair in cells) {
                InventoryCell inventoryCell = pair.Value;

                if (inventoryCell == null) {
                    throw new InvalidOperationException($"Cell {pair.Key} is null.");
                }

                inventoryCell.State = CellState.Empty;
            }

            foreach (IInventoryPlacedItem item in items) {
                indexes.addItem(item);

                foreach (Vector2Int occupiedCell in item.getOccupiedCells()) {
                    if (!cells.TryGetValue(occupiedCell, out InventoryCell inventoryCell) || inventoryCell == null) {
                        throw new InvalidOperationException(
                            $"Item {item.getId()} occupies non-existing cell {occupiedCell}."
                        );
                    }

                    inventoryCell.State = CellState.Occupied;
                }
            }
        }
    }
}