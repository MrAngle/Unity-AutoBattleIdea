using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item.Controller.Api {
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