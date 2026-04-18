using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatQueries {
        Team getTeam();

        Id<CharacterId> getCharacterId();

        string getCharacterName();

        long getMaxHp(); // byc moze to uzywac tylko na UI
        long getCurrentHp(); // byc moze to uzywac tylko na UI

        bool canPlaceItem(EquipItemQuery equipItemQuery);

        ICombatCharacterInventory getInventoryAggregate();
    }
}