using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model;

namespace MageFactory.Item.Api {
    public interface IEntryPointArchetype : IPlaceableItem {
        float getTurnInterval();
        FlowKind getFlowKind();
    }
}