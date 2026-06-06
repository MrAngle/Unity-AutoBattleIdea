using MageFactory.Shared.Event;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Inventory.Api.Event.Dto {
    public readonly struct NewItemPlacedDtoEvent : IDomainEvent {
        public readonly Id<ItemId> placedItemId;
        public readonly ShapeArchetype shapeArchetype;
        public readonly Vector2Int origin;
        public readonly bool isEntryPoint;
        public readonly FlowKind entryPointFlowKind;

        public NewItemPlacedDtoEvent(
            Id<ItemId> placedItemId,
            ShapeArchetype shapeArchetype,
            Vector2Int origin,
            bool isEntryPoint,
            FlowKind entryPointFlowKind) {
            this.placedItemId = placedItemId;
            this.shapeArchetype = shapeArchetype;
            this.origin = origin;
            this.isEntryPoint = isEntryPoint;
            this.entryPointFlowKind = entryPointFlowKind;
        }
    }
}