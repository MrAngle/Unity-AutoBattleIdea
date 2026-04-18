using MageFactory.Shared.Id;

namespace MageFactory.Character.Domain.Service {
    public interface IReadOnlyCharacterData {
        Id<CharacterId> getCharacterId();
        string getCharacterName();
        long getMaxHp();
        long getCurrentHp();
    }
}