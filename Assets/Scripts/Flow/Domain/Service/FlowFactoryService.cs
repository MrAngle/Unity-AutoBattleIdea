using System.Runtime.CompilerServices;
using MageFactory.Flow.Api;
using MageFactory.Flow.Configuration;
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

        public IFlowProcessor create(FlowCreationCommand command) {
            FlowCreationCommand flowCreationCommand = NullGuard.NotNullOrThrow(command);

            return FlowProcessor.create(
                flowCreationCommand.getFlowId(),
                flowCreationCommand.getFlowKind(),
                flowCreationCommand.getStartNode(),
                flowCreationCommand.getRouter(),
                flowCreationCommand.getFlowConsumer(),
                flowCreationCommand.getFlowOwner(),
                flowCreationCommand.getFlowCapabilities(),
                actionContextFactory,
                settings);
        }
    }
}