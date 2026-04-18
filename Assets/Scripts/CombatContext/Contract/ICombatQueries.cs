using MageFactory.CombatContext.Contract.Command;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatQueries {
        IReadOnlyCombatCharacterData getCharacterInfo();

        bool canPlaceItem(EquipItemQuery equipItemQuery);

        ICombatCharacterInventory getInventoryAggregate();
    }
}