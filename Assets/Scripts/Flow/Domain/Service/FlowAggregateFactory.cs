using MageFactory.ActionExecutor.Api;
using MageFactory.Flow.Api;
using MageFactory.FlowRouting;
using MageFactory.Inventory.Api;
using Zenject;

namespace MageFactory.Flow.Domain.Service {
    public sealed class FlowFactory : IFlowFactory {
        private readonly SignalBus _signalBus;
        private readonly IActionExecutor _actionExecutor;

        [Inject]
        public FlowFactory(SignalBus signalBus, IActionExecutor actionExecutor) {
            _signalBus = signalBus;
            _actionExecutor = actionExecutor;
        }

        public IFlowAggregateFacade Create(IPlacedEntryPoint startNode, long power, IFlowRouter router) {
            var flow = (FlowAggregate)FlowAggregate.Create(startNode, power, router, _signalBus, _actionExecutor);

            return flow;
        }
    }
}