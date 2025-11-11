using Combat.ActionExecutor;
using Combat.Flow.Domain.Aggregate;
using Combat.Flow.Domain.Shared;
using Inventory.Items.Domain;
using UI.Combat.Action;

namespace Inventory.EntryPoints {
    public class TickEntryPoint : EntryPointArchetype{
        public TickEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) : 
            base(kind, shapeArchetype, entryPointFactory) {
        }
        
        protected override ActionCommandDescriptor PrepareActionCommandDescriptor(IEntryPointContext entryPointContext) {
            return new ActionCommandDescriptor(
                new AddPower(new DamageToDeal(3))
            );
        } 
    }
}