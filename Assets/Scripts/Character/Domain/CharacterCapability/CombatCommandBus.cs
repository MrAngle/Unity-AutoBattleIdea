using MageFactory.Character.Domain.CombatChar;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatCommandBus : ICombatCommandBus {
        private readonly CombatCharacter combatCharacter;

        public CombatCommandBus(CombatCharacter combatCharacter) {
            this.combatCharacter = combatCharacter;
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return new CombatCharacterEquippedItem(combatCharacter.equipItemOrThrow(item));
        }

        public void combatTick(IFlowConsumer flowConsumer) {
            combatCharacter.combatTick(flowConsumer);
        }

        public void cleanup() {
            combatCharacter.cleanup();
        }

        public void consumeCombatEvent(CombatEvent combatEvent) {
            combatCharacter.consumeCombatEvent(combatEvent);
        }
    }
}