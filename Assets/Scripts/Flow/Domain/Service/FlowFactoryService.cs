using System.Runtime.CompilerServices;
using MageFactory.ActionExecutor.Api;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Flow.Domain.Service {
    public sealed class FlowFactoryService : IFlowFactory {
        private readonly SignalBus signalBus;
        private readonly IActionExecutor actionExecutor;
        private readonly ActionContextFactory actionContextFactory;

        [Inject]
        internal FlowFactoryService(SignalBus injectSignalBus, IActionExecutor injectActionExecutor,
                                    ActionContextFactory injectActionContextFactory) {
            signalBus = NullGuard.NotNullOrThrow(injectSignalBus);
            actionExecutor = NullGuard.NotNullOrThrow(injectActionExecutor);
            actionContextFactory = NullGuard.NotNullOrThrow(injectActionContextFactory);
        }

        public IFlowAggregateFacade create(IFlowItem startNode, IFlowRouter router, IFlowConsumer flowConsumer,
                                           IFlowOwner flowOwner) {
            return FlowAggregate.create(startNode, router, signalBus, actionExecutor, flowConsumer, flowOwner,
                actionContextFactory);
        }
    }
}