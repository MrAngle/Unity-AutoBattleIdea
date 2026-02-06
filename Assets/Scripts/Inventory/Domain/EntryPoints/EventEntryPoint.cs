using MageFactory.ActionEffect;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Domain {
    internal sealed class EventEntryPoint : EntryPointArchetype {
        private EventEntryPoint(ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) : base(
            FlowKind.Defense, shapeArchetype, entryPointFactory) {
        }

        // protected override ActionCommandDescriptor
        //     PrepareActionCommandDescriptor(IEntryPointContext entryPointContext) {
        //     return new ActionCommandDescriptor(
        //         new AddPower(new DamageToReceive(3))
        //     );
        // }

        protected override IEffectsDescriptor prepareEffectsDescriptor(IEntryPointContext entryPointContext) {
            return new EffectsDescription(
                new AddPower(new DamageToReceive(3))
            );
        }
    }
}