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
        private readonly IActionExecutor actionExecutor;
        private readonly ActionContextFactory actionContextFactory;

        [Inject]
        internal FlowFactoryService(IActionExecutor injectActionExecutor,
                                    ActionContextFactory injectActionContextFactory) {
            actionExecutor = NullGuard.NotNullOrThrow(injectActionExecutor);
            actionContextFactory = NullGuard.NotNullOrThrow(injectActionContextFactory);
        }

        public IFlowProcessor create(IFlowItem startNode, IFlowRouter router, IFlowConsumer flowConsumer,
                                     IFlowCapabilities flowCapabilities,
                                     IFlowOwner flowOwner) {
            return FlowProcessor.create(startNode, router, actionExecutor, flowConsumer, flowOwner, flowCapabilities,
                actionContextFactory);
        }
    }
}