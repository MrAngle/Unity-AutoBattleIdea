using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlacedEntryPoint : IInventoryPlacedItem, IInventoryCombatTickableItem {
        public FlowKind getFlowKind();
    }
}