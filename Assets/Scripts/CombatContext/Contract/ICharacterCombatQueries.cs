using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Contract {
    public interface ICharacterCombatQueries {
        IReadOnlyCombatCharacterData getCharacterInfo();

        bool canPlaceItem(EquipItemQuery equipItemQuery);

        ICombatCharacterInventory getInventoryAggregate();
    }
}