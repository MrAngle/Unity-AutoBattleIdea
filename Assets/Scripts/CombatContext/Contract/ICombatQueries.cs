using MageFactory.CombatContext.Contract.Command;
using MageFactory.FlowRouting;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatQueries : IRouterGridAdjacencyActions {
        bool canPlaceItem(EquipItemQuery equipItemQuery);

        ICombatCharacterInventory getInventoryAggregate();
    }
}