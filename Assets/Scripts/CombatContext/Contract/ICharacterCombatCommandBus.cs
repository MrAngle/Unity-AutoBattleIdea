using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContext.Contract {
    public interface ICharacterCombatCommandBus {
        // todo: change name to ICharacterCommandBus
        ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item);

        void combatTick(CombatTicks combatTicks, ICombatCapabilities combatCapabilities);

        void createFlow(Id<ItemId> entryPointItemId, IFlowConsumer flowConsumer,
                        ICombatCapabilities combatCapabilities);

        void cleanup();
        void consumeCombatEvent(CombatEvent combatEvent);
    }
}