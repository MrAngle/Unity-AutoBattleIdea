using System;
using System.Collections.Generic;
using System.Diagnostics;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.BattleManager;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatEvents;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class DefensiveIncomingDamagePerformanceSmokeTest {
        private const int CharacterCount = 12;
        private const int AttackEntryPointCount = 800;
        private const int DefenseEntryPointCount = 1_600;
        private const int TotalItemCount = AttackEntryPointCount + DefenseEntryPointCount;
        private const int StressInventoryWidth = 100;
        private const int StressInventoryHeight = 50;
        private const int MeasuredTicks = 120;
        private const int AttackEntryPointTriggerIntervalTicks = 2;
        private const int AttackPower = 10;
        private const int DefensePowerReduction = -1;

        private const int MinimumExpectedIncomingDamageEvents = AttackEntryPointCount * 10;

        // Regression canary for the current event-triggered defense path with Stability -> Guard -> HP processing.
        private const double MaxIncomingDamageAverageTickMilliseconds = 30.0;

        private static readonly ShapeArchetype AttackPerformanceEntryPointShape = new(
            ShapeArchetypeId.SQUARE_1X1,
            "Incoming Damage Stress Attack Entry Point",
            ItemShape.singleCell());

        private static readonly ShapeArchetype DefensePerformanceEntryPointShape = new(
            ShapeArchetypeId.SQUARE_1X1,
            "Incoming Damage Stress Defense Entry Point",
            ItemShape.singleCell());

        [Test]
        [Category("PerformanceSmoke")]
        public void
            should_process_high_volume_incoming_attack_damage_defense_events_within_smoke_budget() {
            bool previousLogState = Debug.unityLogger.logEnabled;
            Random.State previousRandomState = Random.state;

            try {
                Debug.unityLogger.logEnabled = false;
                Random.InitState(12_345);

                ICombatContext combatContext = BattleScenarioTestHarness.create()
                    .withProductionCombatRuntimeProfile()
                    .withFlowSettings(maxStepsPerSlice: 1)
                    .withInstantFlowItemCastTime()
                    .withMaxInventoryGridDimensions(StressInventoryWidth, StressInventoryHeight)
                    .createContext(createCombatCommands(
                        CharacterCount,
                        AttackEntryPointCount,
                        DefenseEntryPointCount));

                BattleSession session = BattleSessionTestFixtures.basic(combatContext);

                Assert.AreEqual(CharacterCount, combatContext.getAllCharacters().Count);
                Assert.AreEqual(TotalItemCount, countPlacedItems(combatContext));
                Assert.AreEqual(AttackEntryPointCount, countItemsWithShape(
                    combatContext,
                    AttackPerformanceEntryPointShape));
                Assert.AreEqual(DefenseEntryPointCount, countItemsWithShape(
                    combatContext,
                    DefensePerformanceEntryPointShape));
                Assert.AreEqual(0, combatContext.getCombatCapabilities().query().getActiveFlowCount());
                Assert.AreEqual(0, combatContext.getCombatCapabilities().query().getCreatedFlowCount());

                Stopwatch stopwatch = Stopwatch.StartNew();
                session.tickMany(new ManualBattleLoop(), MeasuredTicks);
                stopwatch.Stop();

                int activeFlowsAfterMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getActiveFlowCount();
                int createdFlowsAfterMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getCreatedFlowCount();
                int incomingDamageEventCount = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE);
                int defenderCreatedFlowCount = countCreatedFlowsForTeam(combatContext, Team.TeamB);
                double averageTickMilliseconds = stopwatch.Elapsed.TotalMilliseconds / MeasuredTicks;

                TestContext.WriteLine(
                    $"Defensive incoming damage performance smoke: {CharacterCount} characters, " +
                    $"{AttackEntryPointCount} offensive entry points, " +
                    $"{DefenseEntryPointCount} event defensive entry points, " +
                    $"{incomingDamageEventCount} incoming damage events, " +
                    $"{defenderCreatedFlowCount} defensive flows, " +
                    $"{createdFlowsAfterMeasurement} total created flows, " +
                    $"{activeFlowsAfterMeasurement} active flows after measurement, " +
                    $"{averageTickMilliseconds:0.###} ms/tick average over {MeasuredTicks} measured ticks.");

                Assert.AreEqual(TotalItemCount, countPlacedItems(combatContext));
                Assert.GreaterOrEqual(
                    incomingDamageEventCount,
                    MinimumExpectedIncomingDamageEvents,
                    "Stress scenario did not generate enough incoming attack damage events to exercise defense processing.");
                Assert.GreaterOrEqual(
                    defenderCreatedFlowCount,
                    MinimumExpectedIncomingDamageEvents,
                    "Stress scenario did not create enough event-triggered defensive flows.");
                Assert.Greater(
                    createdFlowsAfterMeasurement,
                    incomingDamageEventCount,
                    "Created flow count should include offensive flows plus event-triggered defensive flows.");
                Assert.LessOrEqual(
                    averageTickMilliseconds,
                    MaxIncomingDamageAverageTickMilliseconds,
                    "This incoming-damage defense smoke is only a threshold for catastrophic hot-path regressions. " +
                    "It exercises flow creation, incoming-damage event dispatch, defensive flow creation, " +
                    "and resolved defensive damage application. If legitimate gameplay growth breaks it, " +
                    "raise the threshold deliberately and keep the new budget documented.");
            }
            finally {
                Random.state = previousRandomState;
                Debug.unityLogger.logEnabled = previousLogState;
            }
        }

        private static CreateCombatCharacterCommand[] createCombatCommands(
            int characterCount,
            int attackEntryPointCount,
            int defenseEntryPointCount) {
            if (characterCount < 2 || characterCount % 2 != 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(characterCount),
                    "Stress scenario needs an even number of at least 2 characters.");
            }

            int attackerCharacterCount = characterCount / 2;
            int defenderCharacterCount = characterCount - attackerCharacterCount;
            List<CreateCombatCharacterCommand> commands = new List<CreateCombatCharacterCommand>(characterCount);
            GridDimensions stressInventoryGridDimensions = new GridDimensions(
                StressInventoryWidth,
                StressInventoryHeight);

            int remainingAttackEntryPoints = attackEntryPointCount;
            for (int characterIndex = 0; characterIndex < attackerCharacterCount; characterIndex++) {
                int charactersLeft = attackerCharacterCount - characterIndex;
                int characterItemCount = remainingAttackEntryPoints / charactersLeft;

                commands.Add(new CreateCombatCharacterCommand(
                    $"Incoming Damage Attack Stress Character {characterIndex}",
                    1_000_000_000,
                    Team.TeamA,
                    stressInventoryGridDimensions,
                    createSparseAttackEntryPointCommands(characterItemCount)));

                remainingAttackEntryPoints -= characterItemCount;
            }

            int remainingDefenseEntryPoints = defenseEntryPointCount;
            for (int characterIndex = 0; characterIndex < defenderCharacterCount; characterIndex++) {
                int charactersLeft = defenderCharacterCount - characterIndex;
                int characterItemCount = remainingDefenseEntryPoints / charactersLeft;

                commands.Add(new CreateCombatCharacterCommand(
                    $"Incoming Damage Defense Stress Character {characterIndex}",
                    1_000_000_000,
                    Team.TeamB,
                    stressInventoryGridDimensions,
                    createSparseDefenseEntryPointCommands(characterItemCount)));

                remainingDefenseEntryPoints -= characterItemCount;
            }

            return commands.ToArray();
        }

        private static EquipItemCommand[] createSparseAttackEntryPointCommands(int count) {
            return createSparseItemCommands(
                count,
                () => new PerformanceAttackEntryPointDefinition());
        }

        private static EquipItemCommand[] createSparseDefenseEntryPointCommands(int count) {
            return createSparseItemCommands(
                count,
                () => new PerformanceDefenseEntryPointDefinition());
        }

        private static EquipItemCommand[] createSparseItemCommands(
            int count,
            Func<IItemDefinition> itemFactory) {
            const int itemStride = 2;

            int maxItemsPerRow = StressInventoryWidth / itemStride;
            int maxItemRows = StressInventoryHeight / itemStride;
            int maxItems = maxItemsPerRow * maxItemRows;

            if (count > maxItems) {
                throw new InvalidOperationException(
                    $"Cannot place {count} sparse test items in {StressInventoryWidth}x{StressInventoryHeight} inventory.");
            }

            List<EquipItemCommand> commands = new List<EquipItemCommand>(count);

            for (int itemIndex = 0; itemIndex < count; itemIndex++) {
                int column = itemIndex % maxItemsPerRow;
                int row = itemIndex / maxItemsPerRow;

                commands.Add(new EquipItemCommand(
                    itemFactory(),
                    new Vector2Int(column * itemStride, row * itemStride)));
            }

            return commands.ToArray();
        }

        private static int countPlacedItems(ICombatContext combatContext) {
            int count = 0;

            foreach (ICombatCharacterFacade character in combatContext.getAllCharacters()) {
                foreach (IGridItemPlaced _ in character.query()
                             .getInventoryAggregate()
                             .getPlacedSnapshot()) {
                    count++;
                }
            }

            return count;
        }

        private static int countItemsWithShape(ICombatContext combatContext, ShapeArchetype shape) {
            int count = 0;

            foreach (ICombatCharacterFacade character in combatContext.getAllCharacters()) {
                foreach (IGridItemPlaced item in character.query()
                             .getInventoryAggregate()
                             .getPlacedSnapshot()) {
                    if (ReferenceEquals(item.getShape(), shape)) {
                        count++;
                    }
                }
            }

            return count;
        }

        private static int countCreatedFlowsForTeam(ICombatContext combatContext, Team team) {
            int count = 0;

            foreach (ICombatCharacterFacade character in combatContext.getAllCharacters()) {
                if (character.query().getCharacterInfo().getTeam() != team) {
                    continue;
                }

                count += character.query().getCreatedFlowsInCurrentBattleCount();
            }

            return count;
        }

        private sealed class PerformanceAttackEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return AttackPerformanceEntryPointShape;
            }

            public IActionDescription getActionDescription() {
                return new PowerDeltaActionDescription(AttackPower);
            }

            public FlowKind getFlowKind() {
                return FlowKind.Damage;
            }

            public EntryPointTriggerKind getTriggerKind() {
                return EntryPointTriggerKind.CombatTick;
            }

            public ICombatHook getCombatHook() {
                return CombatHook.none();
            }

            public CombatTicks getTriggerIntervalTicks() {
                return CombatTicks.of(AttackEntryPointTriggerIntervalTicks);
            }
        }

        private sealed class PerformanceDefenseEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return DefensePerformanceEntryPointShape;
            }

            public IActionDescription getActionDescription() {
                return new PowerDeltaActionDescription(DefensePowerReduction);
            }

            public FlowKind getFlowKind() {
                return FlowKind.Defense;
            }

            public EntryPointTriggerKind getTriggerKind() {
                return EntryPointTriggerKind.CombatEvent;
            }

            public ICombatHook getCombatHook() {
                return CombatHook.onIncomingAttackDamage();
            }

            public CombatTicks getTriggerIntervalTicks() {
                return CombatTicks.ONE;
            }
        }

        private sealed class PowerDeltaActionDescription : IActionDescription {
            private readonly int powerDelta;

            internal PowerDeltaActionDescription(int powerDelta) {
                this.powerDelta = powerDelta;
            }

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(1);
            }

            public IOperations getEffectsDescriptor() {
                return new PowerDeltaOperations(powerDelta);
            }
        }

        private sealed class PowerDeltaOperations : IOperations {
            private readonly IOperation[] effects;

            internal PowerDeltaOperations(int powerDelta) {
                effects = new IOperation[] {
                    new AddPower(DamageRole.ATTACK, new PowerAmount(powerDelta))
                };
            }

            public IReadOnlyList<IOperation> getEffects() {
                return effects;
            }
        }
    }
}