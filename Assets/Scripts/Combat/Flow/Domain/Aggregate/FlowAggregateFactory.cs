using Combat.ActionExecutor;
using Combat.Flow.Domain.Router;
using Inventory.EntryPoints;
using Zenject;

namespace Combat.Flow.Domain.Aggregate {
    public interface IFlowFactory {
        IFlowAggregateFacade Create(PlacedEntryPoint startNode, long power, IFlowRouter router);
    }

    public sealed class FlowFactory : IFlowFactory {
        private readonly SignalBus _signalBus;
        private readonly IActionExecutor _actionExecutor;

        [Inject]
        public FlowFactory(SignalBus signalBus, IActionExecutor actionExecutor) {
            _signalBus = signalBus;
            _actionExecutor = actionExecutor;       
        }

        public IFlowAggregateFacade Create(PlacedEntryPoint startNode, long power, IFlowRouter router) {
            var flow = (FlowAggregate)FlowAggregate.Create(startNode, power, router, _signalBus, _actionExecutor);

            return flow;
        }
    }
}