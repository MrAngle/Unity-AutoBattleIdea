using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;

namespace MageFactory.Flow.Api {
    public interface IFlowFactory {
        IFlowProcessor create(IFlowItem startNode, IFlowRouter router, IFlowConsumer flowConsumer,
                              IFlowOwner flowOwner);
    }
}