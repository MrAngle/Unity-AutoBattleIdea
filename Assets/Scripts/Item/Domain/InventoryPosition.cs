using System.Collections.Generic;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item.Domain {
    public class InventoryPosition : IInventoryPosition {
        private readonly ItemShape _itemShape;

        private readonly HashSet<Vector2Int> _occupiedCells;
        private readonly Vector2Int _origin;

        private InventoryPosition(Vector2Int origin, ItemShape itemItemShape) {
            _origin = origin;
            _itemShape = itemItemShape;
            _occupiedCells = calculateOccupiedCellsByOrigin();
        }

        public static InventoryPosition create(Vector2Int origin, ItemShape itemItemShape) {
            return new InventoryPosition(origin, itemItemShape);
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return _occupiedCells;
        }

        public Vector2Int getOrigin() {
            return _origin;
        }

        private HashSet<Vector2Int> calculateOccupiedCellsByOrigin() {
            return _itemShape.GetCellSetAt(_origin);
        }
    }
}