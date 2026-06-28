using System;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors.PredefinedEvents {
    internal class IncomingAttackDamageCombatEventProcessor {
        private readonly CombatRuntimeSettings combatRuntimeSettings;

        internal IncomingAttackDamageCombatEventProcessor(CombatRuntimeSettings combatRuntimeSettings) {
            this.combatRuntimeSettings = NullGuard.NotNullOrThrow(combatRuntimeSettings);
        }

        public void process(CombatCharacter combatCharacter,
                            IncomingAttackDamageCombatEvent combatEvent,
                            IFlowConsumer flowConsumer,
                            ICombatCapabilities combatCapabilities) {
            NullGuard.NotNullOrThrow(combatCharacter);
            NullGuard.NotNullOrThrow(combatEvent);
            NullGuard.NotNullOrThrow(flowConsumer);
            NullGuard.NotNullOrThrow(combatCapabilities);

            if (!combatEvent.isTargetedAt(combatCharacter.getCharacterInfo().getCharacterId())) {
                throw new InvalidOperationException(
                    $"CombatCharacter '{combatCharacter.getCharacterInfo().getCharacterId()}' " +
                    $"cannot consume event targeted to '{combatEvent.getTargetCharacterId()}'.");
            }

            if (combatCharacter.tryCreateDefensiveFlowFor(combatEvent, flowConsumer, combatCapabilities)) {
                if (combatRuntimeSettings.shouldLogCombatHotPath()) {
                    Debug.Log(
                        $"[Combat] IncomingAttackDamage created defensive flow for character={combatCharacter.getCharacterInfo().getCharacterId()}, " +
                        $"rawDamage={combatEvent.getRawDamageToReceive()}, sourceCharacter={combatEvent.getSourceCharacterId()}");
                }

                return;
            }

            ResolvedDamage resolvedDamage = ResolvedDamage.fromDamageToReceive(combatEvent.getRawDamageToReceive());
            combatCapabilities.command().dispatch(new EnqueueResolvedDamagePacketCombatCommand(
                combatEvent.getTargetCharacterId(),
                combatEvent.getSourceCharacterId(),
                resolvedDamage));

            if (combatRuntimeSettings.shouldLogCombatHotPath()) {
                Debug.Log(
                    $"[Combat] IncomingAttackDamage enqueued resolved damage packet for character={combatCharacter.getCharacterInfo().getCharacterId()}, " +
                    $"rawDamage={combatEvent.getRawDamageToReceive()}, damageRole={combatEvent.getDamageRole()}");
            }
        }
    }
}