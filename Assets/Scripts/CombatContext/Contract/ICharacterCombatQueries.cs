using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatEvents;
using MageFactory.Shared.Id;

namespace MageFactory.CombatContext.Contract {
    public interface ICharacterCombatQueries {
        IReadOnlyCombatCharacterData getCharacterInfo();

        bool canPlaceItem(EquipItemQuery equipItemQuery);

        ICombatCharacterInventory getInventoryAggregate();

        int getActiveFlowCount();

        int getCreatedFlowsInCurrentBattleCount();

        int getActiveFlowCountOnItem(Id<ItemId> itemId);

        int getCombatEventCount(CombatEventType combatEventType);

        int getPreparedGuardCount();

        long getTotalPreparedGuardPower();

        long getCurrentStability();

        long getBaselineStability();

        void collectActiveFlowStates(IActiveFlowStateCollector collector);

        void collectPreparedGuardStates(IPreparedGuardStateCollector collector);
    }
}