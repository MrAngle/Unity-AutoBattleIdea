using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.CombatChar {
    internal sealed class CombatGuardState {
        private const int MaxPreparedGuardPortions = 30;

        private readonly List<PreparedGuard> preparedGuards = new(MaxPreparedGuardPortions);

        internal bool tryAddGuard(GuardPower guardPower, out PreparedGuardAddResult guardAddResult) {
            GuardPower powerToAdd = NullGuard.NotNullOrThrow(guardPower);

            if (powerToAdd.getPower() <= 0) {
                guardAddResult = default;
                return false;
            }

            if (preparedGuards.Count < MaxPreparedGuardPortions) {
                PreparedGuard preparedGuard = createGuard(powerToAdd);
                preparedGuards.Add(preparedGuard);
                guardAddResult = new PreparedGuardAddResult(
                    preparedGuard.toState(),
                    false,
                    default);
                return true;
            }

            PreparedGuardState replacedGuardState = preparedGuards[0].toState();
            preparedGuards.RemoveAt(0);

            PreparedGuard newGuard = createGuard(powerToAdd);
            preparedGuards.Add(newGuard);
            guardAddResult = new PreparedGuardAddResult(
                newGuard.toState(),
                true,
                replacedGuardState);
            return true;
        }

        internal ResolvedDamage applyTo(
            ResolvedDamage resolvedDamage,
            out GuardDamageApplicationResult guardDamageApplicationResult) {
            ResolvedDamage incomingDamage = NullGuard.NotNullOrThrow(resolvedDamage);
            long remainingDamage = incomingDamage.getPower();
            long blockedDamageTotal = 0;
            int destroyedGuardCount = 0;
            bool hasAffectedGuard = false;
            Id<GuardId> firstAffectedGuardId = default;

            if (remainingDamage <= 0 || preparedGuards.Count == 0) {
                guardDamageApplicationResult = new GuardDamageApplicationResult(
                    incomingDamage.getPower(),
                    0,
                    remainingDamage,
                    0,
                    getTotalPreparedGuardPower(),
                    false,
                    default);
                return incomingDamage;
            }

            int guardIndex = 0;
            while (remainingDamage > 0 && guardIndex < preparedGuards.Count) {
                PreparedGuard guard = preparedGuards[guardIndex];
                long blockedDamage = guard.absorb(remainingDamage);
                blockedDamageTotal += blockedDamage;

                if (blockedDamage > 0 && !hasAffectedGuard) {
                    hasAffectedGuard = true;
                    firstAffectedGuardId = guard.getGuardId();
                }

                if (guard.hasNoPower()) {
                    preparedGuards.RemoveAt(guardIndex);
                    destroyedGuardCount++;
                }
                else {
                    guardIndex++;
                }

                if (blockedDamage <= 0) {
                    break;
                }

                remainingDamage -= blockedDamage;
            }

            guardDamageApplicationResult = new GuardDamageApplicationResult(
                incomingDamage.getPower(),
                blockedDamageTotal,
                remainingDamage,
                destroyedGuardCount,
                getTotalPreparedGuardPower(),
                hasAffectedGuard,
                firstAffectedGuardId);
            return new ResolvedDamage(remainingDamage);
        }

        internal int getPreparedGuardCount() {
            return preparedGuards.Count;
        }

        internal long getTotalPreparedGuardPower() {
            long totalPower = 0;

            for (int i = 0; i < preparedGuards.Count; i++) {
                totalPower += preparedGuards[i].getPower();
            }

            return totalPower;
        }

        internal void collectPreparedGuardStates(IPreparedGuardStateCollector collector) {
            IPreparedGuardStateCollector validCollector = NullGuard.NotNullOrThrow(collector);

            for (int i = 0; i < preparedGuards.Count; i++) {
                validCollector.addPreparedGuardState(preparedGuards[i].toState());
            }
        }

        private static PreparedGuard createGuard(GuardPower guardPower) {
            return new PreparedGuard(
                new Id<GuardId>(IdGenerator.Next()),
                guardPower);
        }
    }
}