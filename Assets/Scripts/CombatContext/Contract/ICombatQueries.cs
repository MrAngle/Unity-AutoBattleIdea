using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatQueries : IRouterGridAdjacencyActions, IFlowQueries {
        bool canPlaceItem(EquipItemQuery equipItemQuery);

        ICombatCharacterInventory getInventoryAggregate();
    }
}