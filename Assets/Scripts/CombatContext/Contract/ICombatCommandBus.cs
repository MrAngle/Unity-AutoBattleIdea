using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;

namespace MageFactory.CombatContext.Contract {
    public interface ICombatCommandBus {
        ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item);

        void combatTick(IFlowConsumer flowConsumer);

        void cleanup();
        void consumeCombatEvent(CombatEvent combatEvent);
    }
}