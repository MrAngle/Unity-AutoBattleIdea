using MageFactory.ActionEffect;
using MageFactory.CombatEvents;
using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlacedEntryPoint : IInventoryPlacedItem {
        public FlowKind getFlowKind();

        public EntryPointTriggerKind getTriggerKind();

        public ICombatHook getCombatHook();
    }
}