// using MageFactory.Shared.Model;
// using MageFactory.Shared.Utility;
//
// namespace MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors.PredefinedEvents {
//     internal class ResolvedDamageCombatEventProcessor {
//         public void process(CombatCharacter combatCharacter, DamageResolvedCombatEvent combatEvent)
//         {
//             NullGuard.NotNullOrThrow(combatCharacter);
//             NullGuard.NotNullOrThrow(combatEvent);
//
//             DamageTaken damageTaken = combatCharacter.takeDamage(combatEvent.GetResolvedDamage());
//
//             // opcjonalnie kolejny event
//         }
//     }
//     }
// }

