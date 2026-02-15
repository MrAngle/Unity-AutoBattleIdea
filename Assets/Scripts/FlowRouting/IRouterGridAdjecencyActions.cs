using MageFactory.Flow.Contract;
using UnityEngine;

namespace MageFactory.FlowRouting {
    public interface IRouterGridAdjacencyActions {
        bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item);
    }
}