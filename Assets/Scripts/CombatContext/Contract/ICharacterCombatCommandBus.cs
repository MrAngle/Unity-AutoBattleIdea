using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;

namespace MageFactory.CombatContext.Contract {
    public interface ICharacterCombatCommandBus {
        // todo: change name to ICharacterCommandBus
        ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item);

        void combatTick(IFlowConsumer flowConsumer, ICombatCapabilities combatCapabilities);

        void createFlow(Id<ItemId> entryPointItemId, IFlowConsumer flowConsumer,
                        ICombatCapabilities combatCapabilities);

        void cleanup();
        void consumeCombatEvent(CombatEvent combatEvent);
    }
}