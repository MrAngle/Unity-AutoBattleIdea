using MageFactory.ActionExecutor.Api;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using Zenject;

namespace MageFactory.Flow.Domain.Service {
    public sealed class FlowFactoryService : IFlowFactory {
        private readonly SignalBus signalBus;
        private readonly IActionExecutor actionExecutor;

        [Inject]
        public FlowFactoryService(SignalBus injectSignalBus, IActionExecutor injectActionExecutor) {
            signalBus = injectSignalBus;
            actionExecutor = injectActionExecutor;
        }

        public IFlowAggregateFacade create(IFlowItem startNode, IFlowRouter router, IFlowConsumer flowConsumer,
                                           IFlowOwner flowOwner) {
            return FlowAggregate.create(startNode, router, signalBus, actionExecutor, flowConsumer, flowOwner);
        }
    }
}