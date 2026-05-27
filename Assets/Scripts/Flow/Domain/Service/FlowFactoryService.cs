using System.Runtime.CompilerServices;
using MageFactory.Flow.Api;
using MageFactory.Flow.Configuration;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Flow.Domain.Service {
    public sealed class FlowFactoryService : IFlowFactory {
        private readonly ActionContextFactory actionContextFactory;
        private readonly FlowProcessorSettings settings;

        [Inject]
        internal FlowFactoryService(ActionContextFactory injectActionContextFactory,
                                    FlowProcessorSettings injectSettings) {
            actionContextFactory = NullGuard.NotNullOrThrow(injectActionContextFactory);
            settings = NullGuard.NotNullOrThrow(injectSettings);
        }

        public IFlowProcessor create(FlowKind flowKind,
                                     IFlowItem startNode,
                                     IFlowRouter router,
                                     IFlowConsumer flowConsumer,
                                     IFlowCapabilities flowCapabilities,
                                     IFlowOwner flowOwner) {
            return FlowProcessor.create(flowKind, startNode, router, flowConsumer, flowOwner,
                flowCapabilities,
                actionContextFactory,
                settings);
        }
    }
}