using System;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatEvents {
    public interface ICombatHook {
        CombatHookType getHookType();
        string getPlayerFacingName();
        bool observes(CombatEventType combatEventType);
        bool matches(ICombatEvent combatEvent);
    }

    public sealed class CombatHook : ICombatHook {
        private static readonly CombatHook NoHook = new(
            CombatHookType.None,
            string.Empty,
            null);

        private static readonly CombatHook IncomingAttackDamageHook = new(
            CombatHookType.OnIncomingAttackDamage,
            "OnIncomingAttackDamage",
            CombatEventType.INCOMING_ATTACK_DAMAGE);

        private readonly CombatHookType hookType;
        private readonly string playerFacingName;
        private readonly CombatEventType? observedCombatEventType;

        private CombatHook(
            CombatHookType hookType,
            string playerFacingName,
            CombatEventType? observedCombatEventType) {
            this.hookType = NullGuard.enumDefinedOrThrow(hookType, nameof(hookType));
            this.playerFacingName = NullGuard.NotNullOrThrow(playerFacingName, nameof(playerFacingName));
            this.observedCombatEventType = resolveObservedCombatEventType(
                this.hookType,
                observedCombatEventType);
        }

        private static CombatEventType? resolveObservedCombatEventType(
            CombatHookType hookType,
            CombatEventType? observedCombatEventType) {
            if (hookType == CombatHookType.None) {
                if (observedCombatEventType.HasValue) {
                    throw new ArgumentException(
                        "None combat hook cannot observe a combat event type.",
                        nameof(observedCombatEventType));
                }

                return null;
            }

            CombatEventType eventType = NullGuard.NotNullOrThrow(
                observedCombatEventType,
                nameof(observedCombatEventType));

            return NullGuard.enumDefinedOrThrow(eventType, nameof(observedCombatEventType));
        }

        public static CombatHook none() {
            return NoHook;
        }

        public static CombatHook onIncomingAttackDamage() {
            return IncomingAttackDamageHook;
        }

        public CombatHookType getHookType() {
            return hookType;
        }

        public string getPlayerFacingName() {
            return playerFacingName;
        }

        public bool observes(CombatEventType combatEventType) {
            NullGuard.enumDefinedOrThrow(combatEventType);

            return observedCombatEventType.HasValue
                   && observedCombatEventType.Value == combatEventType;
        }

        public bool matches(ICombatEvent combatEvent) {
            return observes(NullGuard.NotNullOrThrow(combatEvent).getType());
        }
    }

    public enum CombatHookType {
        None,
        OnIncomingAttackDamage
    }
}