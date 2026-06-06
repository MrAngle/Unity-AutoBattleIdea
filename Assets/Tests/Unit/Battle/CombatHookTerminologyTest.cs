using MageFactory.CombatEvents;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using NUnit.Framework;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class CombatHookTerminologyTest {
        [Test]
        public void should_match_on_incoming_attack_damage_hook_to_incoming_attack_damage_fact() {
            IncomingAttackDamageCombatEvent combatEvent = createIncomingAttackDamageEvent();

            ICombatHook hook = CombatHook.onIncomingAttackDamage();

            Assert.AreEqual(CombatHookType.OnIncomingAttackDamage, hook.getHookType());
            Assert.AreEqual("OnIncomingAttackDamage", hook.getPlayerFacingName());
            Assert.IsTrue(hook.observes(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.IsTrue(hook.matches(combatEvent));
        }

        [Test]
        public void should_keep_none_hook_detached_from_combat_facts() {
            IncomingAttackDamageCombatEvent combatEvent = createIncomingAttackDamageEvent();

            ICombatHook hook = CombatHook.none();

            Assert.AreEqual(CombatHookType.None, hook.getHookType());
            Assert.IsFalse(hook.observes(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.IsFalse(hook.matches(combatEvent));
        }

        private static IncomingAttackDamageCombatEvent createIncomingAttackDamageEvent() {
            return new IncomingAttackDamageCombatEvent(
                new Id<CharacterId>(1),
                new Id<CharacterId>(2),
                new DamageToDeal(10));
        }
    }
}