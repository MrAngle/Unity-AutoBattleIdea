using MageFactory.ActionExecutor.Api;
using MageFactory.Flow.Api;
using MageFactory.FlowRouting;
using MageFactory.Inventory.Contract;
using Zenject;

namespace MageFactory.Flow.Domain.Service {
    public sealed class FlowFactoryService : IFlowFactory {
        private readonly SignalBus _signalBus;
        private readonly IActionExecutor _actionExecutor;

        [Inject]
        public FlowFactoryService(SignalBus signalBus, IActionExecutor actionExecutor) {
            _signalBus = signalBus;
            _actionExecutor = actionExecutor;
        }

        public IFlowAggregateFacade create(IInventoryPlacedEntryPoint startNode, long power, IFlowRouter router) {
            var flow = (FlowAggregate)FlowAggregate.create(startNode, power, router, _signalBus, _actionExecutor);

            return flow;
        }
    }
}