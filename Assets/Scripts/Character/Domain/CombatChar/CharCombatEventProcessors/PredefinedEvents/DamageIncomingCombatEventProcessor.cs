using System;
using MageFactory.CombatEvents;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors.PredefinedEvents {
    internal class DamageIncomingCombatEventProcessor {
        public void process(CombatCharacter combatCharacter, DamageIncomingCombatEvent combatEvent) {
            NullGuard.NotNullOrThrow(combatCharacter);
            NullGuard.NotNullOrThrow(combatEvent);

            if (!combatEvent.isTargetedAt(combatCharacter.getCharacterInfo().getCharacterId())) {
                throw new InvalidOperationException(
                    $"CombatCharacter '{combatCharacter.getCharacterInfo().getCharacterId()}' " +
                    $"cannot consume event targeted to '{combatEvent.getTargetCharacterId()}'.");
            }

            // TODO: statistics, effects, modifiers
            ResolvedDamage resolvedDamage = ResolvedDamage.fromDamageToReceive(combatEvent.getRawDamageToReceive());


            DamageTaken damageTaken = combatCharacter.takeDamage(resolvedDamage);

            Debug.Log(
                $"[Combat] DamageIncoming processed for character={combatCharacter.getCharacterInfo().getCharacterId()}, " +
                $"rawDamage={combatEvent.getRawDamageToReceive()}, finalDamage={damageTaken.getPower()}, sourceType={combatEvent.getDamageSourceType()}");
        }
    }
}