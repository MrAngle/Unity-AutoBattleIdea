using System;
using MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors.PredefinedEvents;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors {
    internal class CharacterCombatEventProcessor {
        private readonly IncomingAttackDamageCombatEventProcessor incomingAttackDamageCombatEventProcessor;

        internal CharacterCombatEventProcessor(
            IncomingAttackDamageCombatEventProcessor incomingAttackDamageCombatEventProcessor) {
            this.incomingAttackDamageCombatEventProcessor =
                NullGuard.NotNullOrThrow(incomingAttackDamageCombatEventProcessor);
        }

        internal void process(CombatCharacter combatCharacter,
                              CombatEvent combatEvent,
                              IFlowConsumer flowConsumer,
                              ICombatCapabilities combatCapabilities) {
            NullGuard.NotNullOrThrow(combatCharacter);
            NullGuard.NotNullOrThrow(combatEvent);

            switch (combatEvent) {
                case IncomingAttackDamageCombatEvent incomingAttackDamageCombatEvent:
                    incomingAttackDamageCombatEventProcessor.process(
                        combatCharacter,
                        incomingAttackDamageCombatEvent,
                        flowConsumer,
                        combatCapabilities);
                    return;

                default:
                    throw new NotSupportedException(
                        $"Combat event '{combatEvent.GetType().Name}' is not supported.");
            }
        }
    }
}