using Contracts.Items;
using UnityEngine;

namespace Contracts.Inventory {
    public readonly struct ItemPlacedDtoEvent {
        public long PlacedItemId { get; }
        public ShapeArchetype Data { get; }
        public Vector2Int Origin { get; }

        public ItemPlacedDtoEvent(long id, ShapeArchetype data, Vector2Int origin) {
            PlacedItemId = id;
            Data = data;
            Origin = origin;
        }
    }
}