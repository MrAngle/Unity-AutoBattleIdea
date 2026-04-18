using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface IReadOnlyCombatCharacterData {
        Team getTeam();
        Id<CharacterId> getCharacterId();
        string getCharacterName();
        long getMaxHp(); // byc moze to uzywac tylko na UI
        long getCurrentHp(); // byc moze to uzywac tylko na UI
    }
}