using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Contract {
    public interface IInventoryPlacedEntryPoint : IInventoryPlacedItem {
        public FlowKind getFlowKind();
    }
}