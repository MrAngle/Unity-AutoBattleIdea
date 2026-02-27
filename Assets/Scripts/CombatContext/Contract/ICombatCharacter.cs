using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacter {
        public Id<CharacterId> getId();
        public long getMaxHp();
        public long getCurrentHp();
        string getName();
        void cleanup();
        void combatTick(IFlowConsumer flowConsumer);
        ICharacterCombatCapabilities getCharacterCombatCapabilities();
        Team getTeam();
    }
}