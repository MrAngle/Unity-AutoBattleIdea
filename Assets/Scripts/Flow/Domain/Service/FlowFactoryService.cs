using MageFactory.ActionExecutor.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Api;
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

        public IFlowAggregateFacade create(ICombatCharacterEquippedItem startNode, IFlowRouter router) {
            return FlowAggregate.create(startNode, router, signalBus, actionExecutor);
        }
    }
}