using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;

namespace MageFactory.Flow.Api {
    public interface IFlowFactory {
        IFlowAggregateFacade create(IFlowItem startNode, IFlowRouter router, IFlowConsumer flowConsumer,
                                    IFlowOwner flowOwner);
    }
}