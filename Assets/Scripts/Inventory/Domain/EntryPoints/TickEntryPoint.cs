using MageFactory.ActionEffect;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Domain {
    public class TickEntryPoint : EntryPointArchetype {
        public TickEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) :
            base(kind, shapeArchetype, entryPointFactory) {
        }

        protected override IEffectsDescriptor prepareEffectsDescriptor(IEntryPointContext entryPointContext) {
            return new EffectsDescription(
                new AddPower(new DamageToDeal(3))
            );
        }
    }
}