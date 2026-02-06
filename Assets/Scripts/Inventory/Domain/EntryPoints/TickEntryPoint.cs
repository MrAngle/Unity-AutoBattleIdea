using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Domain {
    public class TickEntryPoint : EntryPointArchetype {
        public TickEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) :
            base(kind, shapeArchetype, entryPointFactory) {
        }
    }
}