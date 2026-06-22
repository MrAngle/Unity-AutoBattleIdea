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
        public readonly FlowPortKind flowPortKind;
        public readonly string flowPortName;
        public readonly string flowPortDescription;

        public NewItemPlacedDtoEvent(
            Id<ItemId> placedItemId,
            ShapeArchetype shapeArchetype,
            Vector2Int origin,
            bool isEntryPoint,
            FlowKind entryPointFlowKind)
            : this(placedItemId, shapeArchetype, origin, isEntryPoint, entryPointFlowKind,
                FlowPortKind.None, string.Empty, string.Empty) {
        }

        public NewItemPlacedDtoEvent(
            Id<ItemId> placedItemId,
            ShapeArchetype shapeArchetype,
            Vector2Int origin,
            bool isEntryPoint,
            FlowKind entryPointFlowKind,
            FlowPortKind flowPortKind,
            string flowPortName,
            string flowPortDescription) {
            this.placedItemId = placedItemId;
            this.shapeArchetype = shapeArchetype;
            this.origin = origin;
            this.isEntryPoint = isEntryPoint;
            this.entryPointFlowKind = entryPointFlowKind;
            this.flowPortKind = flowPortKind;
            this.flowPortName = flowPortName ?? string.Empty;
            this.flowPortDescription = flowPortDescription ?? string.Empty;
        }
    }
}