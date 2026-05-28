using System;
using System.Collections.Generic;
using System.Diagnostics;
using MageFactory.ActionEffect;
using MageFactory.BattleManager;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class CombatTickPerformanceSmokeTest {
        private const int TargetCharacterCount = 12;
        private const int TargetItemCount = 5_000;
        private const int TargetInitialActiveFlowCount = 5_000;
        private const int StressInventoryWidth = 100;

        private const int StressInventoryHeight = 50;

        // Tick is the smallest game time unit; ticks-per-real-second is runtime/GUI execution speed.
        private static readonly int DefaultCombatTicksPerRealSecond =
            BattleSessionSettings.getDefaultCombatTicksPerRealSecond();

        private const int StressSpeedMultiplier = 10;

        private static readonly int StressCombatTicksPerRealSecond =
            DefaultCombatTicksPerRealSecond * StressSpeedMultiplier;

        private const int MeasuredRealSeconds = 10;
        private static readonly int EntryPointTriggerIntervalTicks = StressCombatTicksPerRealSecond;
        private const int WarmupTicks = 5;
        private static readonly int MeasuredTicks = StressCombatTicksPerRealSecond * MeasuredRealSeconds;
        private const double MaxAverageTickMilliseconds = 10.5;

        private static readonly ShapeArchetype PerformanceEntryPointShape = new(
            ShapeArchetypeId.SQUARE_1X1,
            "Performance Entry Point",
            ItemShape.singleCell());

        [Test]
        [Category("PerformanceSmoke")]
        public void
            should_tick_5000_items_and_5000_active_flows_for_10_real_seconds_at_10x_speed_within_smoke_budget() {
            bool previousLogState = Debug.unityLogger.logEnabled;
            Random.State previousRandomState = Random.state;

            try {
                Debug.unityLogger.logEnabled = false;
                Random.InitState(12_345);

                ICombatContext combatContext = BattleScenarioTestHarness.create()
                    .withFlowSettings(maxStepsPerSlice: 1)
                    .withMaxInventoryGridDimensions(StressInventoryWidth, StressInventoryHeight)
                    .createContext(createCombatCommands(TargetCharacterCount, TargetItemCount));

                BattleSession session = BattleSessionTestFixtures.basic(combatContext);
                int placedItemCount = countPlacedItems(combatContext);
                int performanceEntryPointCount = countPerformanceEntryPoints(combatContext);

                Assert.AreEqual(TargetCharacterCount, combatContext.getAllCharacters().Count);
                Assert.AreEqual(TargetItemCount, placedItemCount);
                Assert.AreEqual(TargetItemCount, performanceEntryPointCount);

                session.tickMany(new ManualBattleLoop(), WarmupTicks);

                int initialFlowCommandCount = createInitialFlows(combatContext);

                Assert.AreEqual(TargetInitialActiveFlowCount, initialFlowCommandCount);
                Assert.AreEqual(
                    TargetInitialActiveFlowCount,
                    combatContext.getCombatCapabilities().query().getActiveFlowCount());
                Assert.AreEqual(
                    TargetInitialActiveFlowCount,
                    combatContext.getCombatCapabilities().query().getCreatedFlowCount());

                int activeFlowsBeforeMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getActiveFlowCount();
                int createdFlowsBeforeMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getCreatedFlowCount();

                Assert.AreEqual(TargetInitialActiveFlowCount, activeFlowsBeforeMeasurement);
                Assert.AreEqual(TargetInitialActiveFlowCount, createdFlowsBeforeMeasurement);

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
                double averageTickMilliseconds = stopwatch.Elapsed.TotalMilliseconds / MeasuredTicks;
                int expectedCreatedFlowsAfterMeasurement =
                    TargetInitialActiveFlowCount +
                    TargetItemCount * countTriggerWavesDuringMeasurement(WarmupTicks, MeasuredTicks);

                TestContext.WriteLine(
                    $"Combat tick performance smoke: {TargetCharacterCount} characters, " +
                    $"{TargetItemCount} entry point items, " +
                    $"{activeFlowsBeforeMeasurement} initial active flows, " +
                    $"{createdFlowsAfterMeasurement} created flows after measurement, " +
                    $"{averageTickMilliseconds:0.###} ms/tick average over {MeasuredTicks} measured ticks, " +
                    $"{MeasuredRealSeconds} real seconds under test, " +
                    $"{StressCombatTicksPerRealSecond} ticks/real second target " +
                    $"({DefaultCombatTicksPerRealSecond} default ticks/real second x{StressSpeedMultiplier}).");

                Assert.AreEqual(TargetItemCount, countPlacedItems(combatContext));
                Assert.GreaterOrEqual(activeFlowsAfterMeasurement, TargetInitialActiveFlowCount);
                Assert.AreEqual(expectedCreatedFlowsAfterMeasurement, createdFlowsAfterMeasurement);
                Assert.LessOrEqual(
                    averageTickMilliseconds,
                    MaxAverageTickMilliseconds,
                    "This is only a smoke threshold for catastrophic combat tick regressions. " +
                    "If it fails because of legitimate gameplay growth, raise the threshold deliberately " +
                    "and keep the new budget documented.");
            }
            finally {
                Random.state = previousRandomState;
                Debug.unityLogger.logEnabled = previousLogState;
            }
        }

        private static int createInitialFlows(ICombatContext combatContext) {
            int createdFlowCommandCount = 0;

            foreach (ICombatCharacterFacade character in combatContext.getAllCharacters()) {
                Id<CharacterId> characterId = character.query()
                    .getCharacterInfo()
                    .getCharacterId();

                foreach (IGridItemPlaced item in character.query()
                             .getInventoryAggregate()
                             .getPlacedSnapshot()) {
                    if (!ReferenceEquals(item.getShape(), PerformanceEntryPointShape)) {
                        continue;
                    }

                    combatContext.getCombatCapabilities()
                        .command()
                        .dispatch(new CreateFlowCombatCommand(characterId, item.getId()));
                    createdFlowCommandCount++;
                }
            }

            return createdFlowCommandCount;
        }

        private static CreateCombatCharacterCommand[] createCombatCommands(int characterCount, int itemCount) {
            if (characterCount < 2) {
                throw new ArgumentOutOfRangeException(nameof(characterCount),
                    "Stress scenario needs at least 2 characters.");
            }

            List<CreateCombatCharacterCommand> commands = new List<CreateCombatCharacterCommand>(characterCount);
            int remainingItems = itemCount;
            GridDimensions stressInventoryGridDimensions = new GridDimensions(
                StressInventoryWidth,
                StressInventoryHeight);

            for (int characterIndex = 0; characterIndex < characterCount; characterIndex++) {
                int charactersLeft = characterCount - characterIndex;
                int characterItemCount = remainingItems / charactersLeft;
                Team team = characterIndex < characterCount / 2
                    ? Team.TeamA
                    : Team.TeamB;

                commands.Add(new CreateCombatCharacterCommand(
                    $"Performance Character {characterIndex}",
                    1_000_000_000,
                    team,
                    stressInventoryGridDimensions,
                    createPackedEntryPointCommands(characterItemCount)));

                remainingItems -= characterItemCount;
            }

            return commands.ToArray();
        }

        private static EquipItemCommand[] createPackedEntryPointCommands(int count) {
            if (count > StressInventoryWidth * StressInventoryHeight) {
                throw new InvalidOperationException(
                    $"Cannot place {count} test items in {StressInventoryWidth}x{StressInventoryHeight} inventory.");
            }

            List<EquipItemCommand> commands = new List<EquipItemCommand>(count);

            for (int itemIndex = 0; itemIndex < count; itemIndex++) {
                commands.Add(new EquipItemCommand(
                    new PerformanceEntryPointDefinition(),
                    new Vector2Int(
                        itemIndex % StressInventoryWidth,
                        itemIndex / StressInventoryWidth)));
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

        private static int countPerformanceEntryPoints(ICombatContext combatContext) {
            int count = 0;

            foreach (ICombatCharacterFacade character in combatContext.getAllCharacters()) {
                foreach (IGridItemPlaced item in character.query()
                             .getInventoryAggregate()
                             .getPlacedSnapshot()) {
                    if (ReferenceEquals(item.getShape(), PerformanceEntryPointShape)) {
                        count++;
                    }
                }
            }

            return count;
        }

        private static int countTriggerWavesDuringMeasurement(int warmupTicks, int measuredTicks) {
            int triggerWavesBeforeMeasurement = warmupTicks / EntryPointTriggerIntervalTicks;
            int triggerWavesAfterMeasurement = (warmupTicks + measuredTicks) / EntryPointTriggerIntervalTicks;

            return triggerWavesAfterMeasurement - triggerWavesBeforeMeasurement;
        }

        private sealed class PerformanceEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return PerformanceEntryPointShape;
            }

            public IActionDescription getActionDescription() {
                return NoOpActionDescription.Instance;
            }

            public FlowKind getFlowKind() {
                return FlowKind.Damage;
            }

            public CombatTicks getTriggerIntervalTicks() {
                return CombatTicks.of(EntryPointTriggerIntervalTicks);
            }
        }

        private sealed class NoOpActionDescription : IActionDescription {
            internal static readonly NoOpActionDescription Instance = new();

            public Duration getCastTime() {
                return new Duration(0);
            }

            public IOperations getEffectsDescriptor() {
                return NoOpOperations.Instance;
            }
        }

        private sealed class NoOpOperations : IOperations {
            internal static readonly NoOpOperations Instance = new();
            private static readonly IOperation[] NoEffects = Array.Empty<IOperation>();

            public IReadOnlyList<IOperation> getEffects() {
                return NoEffects;
            }
        }
    }
}