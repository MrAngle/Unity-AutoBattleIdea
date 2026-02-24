using MageFactory.Shared.Id;

namespace MageFactory.CombatContext.Contract {
    public interface IReadCombatContext {
        public bool tryGetRandomEnemyOf(Id<CharacterId> sourceId, out ICombatCharacter enemy);
    }
}