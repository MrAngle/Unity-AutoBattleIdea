using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using UnityEngine;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatQueries : IRouterGridAdjacencyActions {
        bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item);

        bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item);
    }
}