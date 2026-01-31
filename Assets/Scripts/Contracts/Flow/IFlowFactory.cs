using Contracts.Inventory;

namespace Contracts.Flow {
    public interface IFlowFactory {
        IFlowAggregateFacade Create(IPlacedEntryPoint startNode, long power, IFlowRouter router);
    }
}