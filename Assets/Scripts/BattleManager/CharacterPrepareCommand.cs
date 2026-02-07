using MageFactory.Shared.Model;

namespace MageFactory.BattleManager {
    public record CharacterPrepareCommand(
        string name,
        int maxHp,
        Team team);
}