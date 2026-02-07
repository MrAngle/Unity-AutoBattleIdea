using MageFactory.Item.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Domain {
    public class TickEntryPoint : EntryPointArchetype {
        public TickEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) :
            base(kind, shapeArchetype, entryPointFactory) {
        }
    }
}