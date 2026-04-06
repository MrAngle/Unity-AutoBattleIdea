using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MageFactory.Inventory.Contract;
using MageFactory.Shared.Id;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    internal class InventoryRegistryIndexes {
        private readonly Dictionary<Vector2Int, IInventoryPlacedItem> cellToItemIndex = new();
        private readonly Dictionary<Id<ItemId>, IInventoryPlacedItem> itemIdToItemIndex = new();
        private readonly HashSet<IInventoryPlacedEntryPoint> entryPointsIndex = new();

        internal void clear() {
            cellToItemIndex.Clear();
            itemIdToItemIndex.Clear();
            entryPointsIndex.Clear();
        }

        internal void addItem(IInventoryPlacedItem item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            itemIdToItemIndex[item.getId()] = item;

            foreach (Vector2Int occupiedCell in item.getOccupiedCells()) {
                cellToItemIndex[occupiedCell] = item;
            }

            if (item is IInventoryPlacedEntryPoint entryPoint) {
                entryPointsIndex.Add(entryPoint);
            }
        }

        internal void removeItem(IInventoryPlacedItem item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            itemIdToItemIndex.Remove(item.getId());

            foreach (Vector2Int occupiedCell in item.getOccupiedCells()) {
                if (cellToItemIndex.TryGetValue(occupiedCell, out IInventoryPlacedItem mappedItem)
                    && ReferenceEquals(mappedItem, item)) {
                    cellToItemIndex.Remove(occupiedCell);
                }
            }

            if (item is IInventoryPlacedEntryPoint entryPoint) {
                entryPointsIndex.Remove(entryPoint);
            }
        }

        internal void updateItem(IInventoryPlacedItem item, IEnumerable<Vector2Int> oldOccupiedCells) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            if (oldOccupiedCells == null) {
                throw new ArgumentNullException(nameof(oldOccupiedCells));
            }

            foreach (Vector2Int oldOccupiedCell in oldOccupiedCells) {
                if (cellToItemIndex.TryGetValue(oldOccupiedCell, out IInventoryPlacedItem mappedItem)
                    && ReferenceEquals(mappedItem, item)) {
                    cellToItemIndex.Remove(oldOccupiedCell);
                }
            }

            itemIdToItemIndex[item.getId()] = item;

            foreach (Vector2Int newOccupiedCell in item.getOccupiedCells()) {
                cellToItemIndex[newOccupiedCell] = item;
            }

            if (item is IInventoryPlacedEntryPoint entryPoint) {
                entryPointsIndex.Add(entryPoint);
            }
        }

        internal bool tryGetItem(Id<ItemId> itemId, out IInventoryPlacedItem item) {
            return itemIdToItemIndex.TryGetValue(itemId, out item);
        }

        internal bool tryGetItemAtCell(Vector2Int cell, out IInventoryPlacedItem item) {
            return cellToItemIndex.TryGetValue(cell, out item);
        }

        internal bool isCellOccupied(Vector2Int cell) {
            return cellToItemIndex.ContainsKey(cell);
        }

        internal IReadOnlyCollection<IInventoryPlacedEntryPoint> getEntryPoints() {
            return entryPointsIndex;
        }

        internal IReadOnlyDictionary<Vector2Int, IInventoryPlacedItem> getCellToItem() {
            return new ReadOnlyDictionary<Vector2Int, IInventoryPlacedItem>(cellToItemIndex);
        }
    }
}