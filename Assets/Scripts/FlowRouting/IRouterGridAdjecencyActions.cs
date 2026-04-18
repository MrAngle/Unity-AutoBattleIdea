using MageFactory.Flow.Contract;
using UnityEngine;

namespace MageFactory.FlowRouting {
    public delegate bool TryGetFlowItemAtCell(Vector2Int cell, out IFlowItem item);

    // public interface IRouterGridAdjacencyActions {
    //     bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item);
    // }
}