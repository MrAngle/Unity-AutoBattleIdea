using MageFactory.Item.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Domain.EntryPoint {
    internal class TickEntryPoint : EntryPointArchetype {
        internal TickEntryPoint(FlowKind flowKind, ShapeArchetype shapeArchetype,
            IEntryPointFactory entryPointFactory) :
            base(flowKind, shapeArchetype, entryPointFactory) {
        }
    }
}