using System;
using MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors.PredefinedEvents;
using MageFactory.CombatEvents;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors {
    internal class CharacterCombatEventProcessor {
        private readonly DamageIncomingCombatEventProcessor damageIncomingCombatEventProcessor;

        internal CharacterCombatEventProcessor(DamageIncomingCombatEventProcessor damageIncomingCombatEventProcessor) {
            this.damageIncomingCombatEventProcessor =
                NullGuard.NotNullOrThrow(damageIncomingCombatEventProcessor);
        }

        public void process(CombatCharacter combatCharacter, CombatEvent combatEvent) {
            NullGuard.NotNullOrThrow(combatCharacter);
            NullGuard.NotNullOrThrow(combatEvent);

            switch (combatEvent) {
                case DamageIncomingCombatEvent damageIncomingCombatEvent:
                    damageIncomingCombatEventProcessor.process(combatCharacter, damageIncomingCombatEvent);
                    return;

                default:
                    throw new NotSupportedException(
                        $"Combat event '{combatEvent.GetType().Name}' is not supported.");
            }
        }
    }
}