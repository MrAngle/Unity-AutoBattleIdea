using MageFactory.FlowRouting;
using MageFactory.Item.Controller.Api;

namespace MageFactory.Flow.Api {
    public interface IFlowFactory {
        IFlowAggregateFacade create(IPlacedEntryPoint startNode, long power, IFlowRouter router);
    }
}