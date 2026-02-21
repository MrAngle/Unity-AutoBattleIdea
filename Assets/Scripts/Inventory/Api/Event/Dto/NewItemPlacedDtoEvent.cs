using MageFactory.Shared.Event;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Api.Event.Dto {
    public readonly struct NewItemPlacedDtoEvent : IDomainEvent {
        public readonly long placedItemId;
        public readonly ShapeArchetype shapeArchetype;
        public readonly Vector2Int origin;

        public NewItemPlacedDtoEvent(
            long placedItemId,
            ShapeArchetype shapeArchetype,
            Vector2Int origin) {
            this.placedItemId = placedItemId;
            this.shapeArchetype = shapeArchetype;
            this.origin = origin;
        }
    }
}