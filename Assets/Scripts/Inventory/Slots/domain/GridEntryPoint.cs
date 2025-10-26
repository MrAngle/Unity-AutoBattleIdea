using Combat.Flow.Domain.Aggregate;
using Shared.Utility;
using UnityEngine;

namespace Inventory.Slots.Domain {
    
    public sealed class GridEntryPointId : StrongId<GridEntryPointId> {}

    public readonly struct GridEntryPoint
    {

        public FlowKind Kind { get; }
        public Vector2Int Position { get; }

        public GridEntryPoint(FlowKind kind, Vector2Int position) {
            Kind = kind;
            Position = position;
        }

        public override string ToString() => $"({Kind})@{Position}";
    }
}