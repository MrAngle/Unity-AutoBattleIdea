using System.Collections.Generic;
using MageFactory.Inventory.Api;
using UnityEngine;

namespace MageFactory.Inventory.Domain {
    public class InventoryPosition : IInventoryPosition {
        private readonly ItemShape _itemShape;

        private readonly HashSet<Vector2Int> _occupiedCells;
        private readonly Vector2Int _origin;

        private InventoryPosition(Vector2Int origin, ItemShape itemItemShape) {
            _origin = origin;
            _itemShape = itemItemShape;
            _occupiedCells = CalculateOccupiedCellsByOrigin();
        }

        public IReadOnlyCollection<Vector2Int> GetOccupiedCells() {
            return _occupiedCells;
        }

        public Vector2Int GetOrigin() {
            return _origin;
        }

        public static InventoryPosition Create(Vector2Int origin, ItemShape itemItemShape) {
            return new InventoryPosition(origin, itemItemShape);
        }

        private HashSet<Vector2Int> CalculateOccupiedCellsByOrigin() {
            return _itemShape.GetCellSetAt(_origin);
        }
    }
}