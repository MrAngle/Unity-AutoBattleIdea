using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Id;

namespace MageFactory.CombatContext.Contract {
    public interface ICharacterCombatQueries {
        IReadOnlyCombatCharacterData getCharacterInfo();

        bool canPlaceItem(EquipItemQuery equipItemQuery);

        ICombatCharacterInventory getInventoryAggregate();

        int getActiveFlowCount();

        int getCreatedFlowsInCurrentBattleCount();

        int getActiveFlowCountOnItem(Id<ItemId> itemId);

        void collectActiveFlowStates(IActiveFlowStateCollector collector);
    }
}