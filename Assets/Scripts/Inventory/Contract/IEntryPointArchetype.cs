using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Contract {
    public interface IEntryPointArchetype : IInventoryPlaceableItem {
        FlowKind getFlowKind();
    }
}