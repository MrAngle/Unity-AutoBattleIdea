using Combat.Flow.Domain.Router;
using Inventory.EntryPoints;
using Zenject;

namespace Combat.Flow.Domain.Aggregate {
    public interface IFlowFactory {
        IFlowAggregateFacade Create(PlacedEntryPoint startNode, long power, IFlowRouter router);
    }

    public sealed class FlowFactory : IFlowFactory {
        // private readonly IGridInspector _gridInspector;
        private readonly SignalBus _signalBus;

        [Inject]
        public FlowFactory(SignalBus signalBus) {
            // _gridInspector = gridInspector;
            _signalBus = signalBus;
        }

        public IFlowAggregateFacade Create(PlacedEntryPoint startNode, long power, IFlowRouter router) {
            // IFlowRouter router = GridAdjacencyRouter.Create(_gridInspector);
            var flow = (FlowAggregate)FlowAggregate.Create(startNode, power, router, _signalBus);

            // Bridge domena -> UI (opcjonalnie)
            // ((IFlowEvents)flow).OnPowerDeltaApplied += flowPowerDeltaApplied =>
            // {
            //     _signalBus.Fire(new ItemPowerChangedDtoEvent(
            //         flowPowerDeltaApplied.SourceItemId, 
            //         flowPowerDeltaApplied.Delta, 
            //         flowPowerDeltaApplied.SourceOrigin));
            // };

            return flow;
        }
    }
}