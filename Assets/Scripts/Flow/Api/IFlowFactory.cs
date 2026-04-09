using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Api {
    public interface IFlowFactory {
        IFlowProcessor create(FlowKind flowKind,
                              IFlowItem startNode,
                              IFlowRouter router,
                              IFlowConsumer flowConsumer,
                              IFlowCapabilities flowCapabilities,
                              IFlowOwner flowOwner);
    }
}