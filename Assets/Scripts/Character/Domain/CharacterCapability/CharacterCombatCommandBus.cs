using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CharacterCombatCommandBus : ICharacterCombatCommandBus {
        private readonly CombatCharacter combatCharacter;

        public CharacterCombatCommandBus(CombatCharacter combatCharacter) {
            this.combatCharacter = combatCharacter;
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return new CombatCharacterEquippedItem(combatCharacter.equipItemOrThrow(item));
        }

        public void combatTick(CombatTicks combatTicks, ICombatCapabilities combatCapabilities) {
            combatCharacter.combatTick(combatTicks, combatCapabilities);
        }

        public void createFlow(Id<ItemId> entryPointItemId, IFlowConsumer flowConsumer,
                               ICombatCapabilities combatCapabilities) {
            combatCharacter.createFlow(entryPointItemId, flowConsumer, combatCapabilities);
        }

        public void cleanup() {
            combatCharacter.cleanup();
        }

        public void consumeCombatEvent(CombatEvent combatEvent) {
            combatCharacter.consumeCombatEvent(combatEvent);
        }
    }
}