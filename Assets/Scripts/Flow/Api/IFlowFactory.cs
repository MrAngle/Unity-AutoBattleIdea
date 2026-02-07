using MageFactory.FlowRouting;
using MageFactory.Inventory.Contract;

namespace MageFactory.Flow.Api {
    public interface IFlowFactory {
        IFlowAggregateFacade create(IInventoryPlacedEntryPoint startNode, long power, IFlowRouter router);
    }
}