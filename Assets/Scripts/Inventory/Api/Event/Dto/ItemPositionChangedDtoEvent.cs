using MageFactory.Shared.Event;
using MageFactory.Shared.Id;
using UnityEngine;

namespace MageFactory.Inventory.Api.Event.Dto {
    public readonly struct ItemPositionChangedDtoEvent : IDomainEvent {
        public readonly Id<ItemId> placedItemId;
        public readonly Vector2Int newOriginPosition;

        public ItemPositionChangedDtoEvent(
            Id<ItemId> placedItemId,
            Vector2Int newOriginPosition) {
            this.placedItemId = placedItemId;
            this.newOriginPosition = newOriginPosition;
        }
    }
}