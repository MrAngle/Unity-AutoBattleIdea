using MageFactory.Inventory.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Domain.EntryPoint {
    internal class TickEntryPoint : EntryPointArchetype {
        internal TickEntryPoint(FlowKind flowKind, ShapeArchetype shapeArchetype,
            IEntryPointFactory entryPointFactory) :
            base(flowKind, shapeArchetype, entryPointFactory) {
        }

        internal static IEntryPointArchetype create(IEntryPointDefinition entryPointDefinition,
            IEntryPointFactory entryPointFactory) {
            return new TickEntryPoint(entryPointDefinition.getFlowKind(), entryPointDefinition.getShape(),
                entryPointFactory);
        }
    }
}