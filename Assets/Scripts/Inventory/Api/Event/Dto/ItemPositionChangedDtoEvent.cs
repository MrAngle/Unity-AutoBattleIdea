using MageFactory.Shared.Event;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Api.Event.Dto {
    public readonly struct ItemPositionChangedDtoEvent : IDomainEvent {
        public readonly Id<ItemId> placedItemId;
        public readonly ShapeArchetype shapeArchetype;
        public readonly Vector2Int newOriginPosition;
        public readonly Vector2Int oldOriginPosition;

        public ItemPositionChangedDtoEvent(
            Id<ItemId> placedItemId,
            ShapeArchetype shapeArchetype,
            Vector2Int newOriginPosition,
            Vector2Int oldOriginPosition) {
            this.placedItemId = placedItemId;
            this.shapeArchetype = shapeArchetype;
            this.newOriginPosition = newOriginPosition;
            this.oldOriginPosition = oldOriginPosition;
        }
    }
}