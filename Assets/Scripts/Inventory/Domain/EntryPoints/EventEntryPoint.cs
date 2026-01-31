using Contracts.Actionexe;
using Contracts.Flow;
using Contracts.Items;

namespace Inventory.EntryPoints {
    internal sealed class EventEntryPoint : EntryPointArchetype {
        private EventEntryPoint(ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) : base(
            FlowKind.Defense, shapeArchetype, entryPointFactory) {
        }

        protected override ActionCommandDescriptor
            PrepareActionCommandDescriptor(IEntryPointContext entryPointContext) {
            return new ActionCommandDescriptor(
                new AddPower(new DamageToReceive(3))
            );
        }
    }
}