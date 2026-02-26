using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCharacter {
        public Id<CharacterId> getId();
        ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item);
        bool canPlaceItem(EquipItemQuery equipItemQuery);
        ICombatCharacterInventory getInventoryAggregate();
        public long getMaxHp();
        public long getCurrentHp();
        void apply(PowerAmount powerAmount);
        string getName();
        void cleanup();
        void combatTick(IFlowConsumer flowConsumer);
        ICharacterCombatCapabilities getCharacterCombatCapabilities();
        Team getTeam();
    }
}