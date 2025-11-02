using Combat.Flow.Domain.Aggregate;
using Combat.Flow.Domain.Router;
using UnityEngine;
using Zenject;

namespace Inventory.EntryPoints {
    
    public interface IEntryPointFactory
    {
        IPlacedEntryPoint CreatePlacedEntryPoint(FlowKind kind, Vector2Int position, IGridInspector gridInspector);
    }
    
    public class EntryPointFactory : IEntryPointFactory {
        private readonly IFlowFactory _flowFactory;

        [Inject]
        public EntryPointFactory(IFlowFactory flowFactory)
        {
            _flowFactory = flowFactory;
        }

        public IPlacedEntryPoint CreatePlacedEntryPoint(FlowKind kind, Vector2Int position, IGridInspector gridInspector)
        {
            IPlacedEntryPoint placedEntryPoint = PlacedEntryPoint.Create(kind, position, gridInspector, _flowFactory);

            return placedEntryPoint;
        }
        
        
    }
}