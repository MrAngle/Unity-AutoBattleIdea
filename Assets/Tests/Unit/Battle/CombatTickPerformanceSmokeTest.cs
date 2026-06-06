using System;
using System.Collections.Generic;
using System.Diagnostics;
using MageFactory.ActionEffect;
using MageFactory.BattleManager;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
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
        private const int RoutingPairCount = 800;
        private const int RoutingItemCount = RoutingPairCount * 2;
        private const int StressInventoryWidth = 100;

        private const int StressInventoryHeight = 50;
        private const int RoutingMeasuredTicks = 200;
        private const int RoutingEntryPointTriggerIntervalTicks = 2;

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
        private static readonly int PerformanceFlowCastTicks = WarmupTicks + MeasuredTicks + 10_000;
        private const double MaxSteadyCastAverageTickMilliseconds = 12.0;
        private const double MaxRoutingAverageTickMilliseconds = 25.0;

        private static readonly ShapeArchetype PerformanceEntryPointShape = new(
            ShapeArchetypeId.SQUARE_1X1,
            "Performance Entry Point",
            ItemShape.singleCell());

        private static readonly ShapeArchetype RoutingProcessorShape = new(
            ShapeArchetypeId.SQUARE_1X1,
            "Performance Routing Processor",
            ItemShape.singleCell());

        [Test]
        [Category("PerformanceSmoke")]
        public void
            should_tick_5000_long_casting_flows_without_resolution_within_smoke_budget() {
            bool previousLogState = Debug.unityLogger.logEnabled;
            Random.State previousRandomState = Random.state;

            try {
                Debug.unityLogger.logEnabled = false;
                Random.InitState(12_345);

                ICombatContext combatContext = BattleScenarioTestHarness.create()
                    .withProductionCombatRuntimeProfile()
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
                TestContext.WriteLine(
                    $"Combat steady-casting performance smoke: {TargetCharacterCount} characters, " +
                    $"{TargetItemCount} entry point items, " +
                    $"{activeFlowsBeforeMeasurement} initial active flows, " +
                    $"{createdFlowsAfterMeasurement} created flows after measurement, " +
                    $"{averageTickMilliseconds:0.###} ms/tick average over {MeasuredTicks} measured ticks, " +
                    $"{MeasuredRealSeconds} real seconds under test, " +
                    $"{StressCombatTicksPerRealSecond} ticks/real second target " +
                    $"({DefaultCombatTicksPerRealSecond} default ticks/real second x{StressSpeedMultiplier}).");

                Assert.AreEqual(TargetItemCount, countPlacedItems(combatContext));
                Assert.AreEqual(TargetInitialActiveFlowCount, activeFlowsAfterMeasurement);
                Assert.AreEqual(TargetInitialActiveFlowCount, createdFlowsAfterMeasurement);
                Assert.LessOrEqual(
                    averageTickMilliseconds,
                    MaxSteadyCastAverageTickMilliseconds,
                    "This steady-casting smoke is only a threshold for catastrophic active-flow tick regressions. " +
                    "It intentionally avoids routing and flow resolution. " +
                    "If it fails because of legitimate gameplay growth, raise the threshold deliberately " +
                    "and keep the new budget documented.");
            }
            finally {
                Random.state = previousRandomState;
                Debug.unityLogger.logEnabled = previousLogState;
            }
        }

        [Test]
        [Category("PerformanceSmoke")]
        public void
            should_create_route_resolve_and_recreate_flows_within_smoke_budget() {
            bool previousLogState = Debug.unityLogger.logEnabled;
            Random.State previousRandomState = Random.state;

            try {
                Debug.unityLogger.logEnabled = false;
                Random.InitState(12_345);

                ICombatContext combatContext = BattleScenarioTestHarness.create()
                    .withProductionCombatRuntimeProfile()
                    .withFlowSettings(maxStepsPerSlice: 1)
                    .withMaxInventoryGridDimensions(StressInventoryWidth, StressInventoryHeight)
                    .createContext(createRoutingCombatCommands(TargetCharacterCount, RoutingPairCount));

                BattleSession session = BattleSessionTestFixtures.basic(combatContext);

                Assert.AreEqual(TargetCharacterCount, combatContext.getAllCharacters().Count);
                Assert.AreEqual(RoutingItemCount, countPlacedItems(combatContext));
                Assert.AreEqual(RoutingPairCount, countPerformanceEntryPoints(combatContext));
                Assert.AreEqual(0, combatContext.getCombatCapabilities().query().getActiveFlowCount());
                Assert.AreEqual(0, combatContext.getCombatCapabilities().query().getCreatedFlowCount());

                Stopwatch stopwatch = Stopwatch.StartNew();
                session.tickMany(new ManualBattleLoop(), RoutingMeasuredTicks);
                stopwatch.Stop();

                int activeFlowsAfterMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getActiveFlowCount();
                int createdFlowsAfterMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getCreatedFlowCount();
                int expectedCreatedFlows =
                    RoutingPairCount * (RoutingMeasuredTicks / RoutingEntryPointTriggerIntervalTicks);
                double averageTickMilliseconds = stopwatch.Elapsed.TotalMilliseconds / RoutingMeasuredTicks;

                TestContext.WriteLine(
                    $"Combat routing/resolution performance smoke: {TargetCharacterCount} characters, " +
                    $"{RoutingPairCount} entry->processor pairs, " +
                    $"{createdFlowsAfterMeasurement} created flows after measurement, " +
                    $"{activeFlowsAfterMeasurement} active flows after measurement, " +
                    $"{averageTickMilliseconds:0.###} ms/tick average over {RoutingMeasuredTicks} measured ticks.");

                Assert.AreEqual(RoutingItemCount, countPlacedItems(combatContext));
                Assert.AreEqual(RoutingPairCount, activeFlowsAfterMeasurement);
                Assert.AreEqual(expectedCreatedFlows, createdFlowsAfterMeasurement);
                Assert.LessOrEqual(
                    averageTickMilliseconds,
                    MaxRoutingAverageTickMilliseconds,
                    "This routing/resolution smoke is only a threshold for catastrophic combat tick regressions. " +
                    "It intentionally exercises flow creation, cast completion, routing, slot release, " +
                    "and flow consumption. If it fails because of legitimate gameplay growth, raise the " +
                    "threshold deliberately and keep the new budget documented.");
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

        private static CreateCombatCharacterCommand[] createRoutingCombatCommands(int characterCount, int pairCount) {
            if (characterCount < 2) {
                throw new ArgumentOutOfRangeException(nameof(characterCount),
                    "Stress scenario needs at least 2 characters.");
            }

            List<CreateCombatCharacterCommand> commands = new List<CreateCombatCharacterCommand>(characterCount);
            int remainingPairs = pairCount;
            GridDimensions stressInventoryGridDimensions = new GridDimensions(
                StressInventoryWidth,
                StressInventoryHeight);

            for (int characterIndex = 0; characterIndex < characterCount; characterIndex++) {
                int charactersLeft = characterCount - characterIndex;
                int characterPairCount = remainingPairs / charactersLeft;
                Team team = characterIndex < characterCount / 2
                    ? Team.TeamA
                    : Team.TeamB;

                commands.Add(new CreateCombatCharacterCommand(
                    $"Routing Performance Character {characterIndex}",
                    1_000_000_000,
                    team,
                    stressInventoryGridDimensions,
                    createPackedRoutingPairCommands(characterPairCount)));

                remainingPairs -= characterPairCount;
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

        private static EquipItemCommand[] createPackedRoutingPairCommands(int pairCount) {
            // Keep pairs isolated so routing measures entry->processor work, not accidental neighbor chains.
            const int pairStrideX = 3;
            const int pairStrideY = 2;

            int maxPairsPerRow = StressInventoryWidth / pairStrideX;
            int maxPairRows = StressInventoryHeight / pairStrideY;
            int maxPairs = maxPairsPerRow * maxPairRows;

            if (pairCount > maxPairs) {
                throw new InvalidOperationException(
                    $"Cannot place {pairCount} test pairs in {StressInventoryWidth}x{StressInventoryHeight} inventory.");
            }

            List<EquipItemCommand> commands = new List<EquipItemCommand>(pairCount * 2);

            for (int pairIndex = 0; pairIndex < pairCount; pairIndex++) {
                int pairColumn = pairIndex % maxPairsPerRow;
                int pairRow = pairIndex / maxPairsPerRow;
                int entryX = pairColumn * pairStrideX;
                int row = pairRow * pairStrideY;

                commands.Add(new EquipItemCommand(
                    new RoutingPerformanceEntryPointDefinition(),
                    new Vector2Int(entryX, row)));
                commands.Add(new EquipItemCommand(
                    new RoutingPerformanceProcessorDefinition(),
                    new Vector2Int(entryX + 1, row)));
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

            public EntryPointTriggerKind getTriggerKind() {
                return EntryPointTriggerKind.CombatTick;
            }

            public ICombatHook getCombatHook() {
                return CombatHook.none();
            }

            public CombatTicks getTriggerIntervalTicks() {
                return CombatTicks.of(EntryPointTriggerIntervalTicks);
            }
        }

        private sealed class RoutingPerformanceEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return PerformanceEntryPointShape;
            }

            public IActionDescription getActionDescription() {
                return RoutingNoOpActionDescription.Instance;
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
                return CombatTicks.of(RoutingEntryPointTriggerIntervalTicks);
            }
        }

        private sealed class RoutingPerformanceProcessorDefinition : IItemDefinition {
            public ShapeArchetype getShape() {
                return RoutingProcessorShape;
            }

            public IActionDescription getActionDescription() {
                return RoutingNoOpActionDescription.Instance;
            }
        }

        private sealed class NoOpActionDescription : IActionDescription {
            internal static readonly NoOpActionDescription Instance = new();

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(PerformanceFlowCastTicks);
            }

            public IOperations getEffectsDescriptor() {
                return NoOpOperations.Instance;
            }
        }

        private sealed class RoutingNoOpActionDescription : IActionDescription {
            internal static readonly RoutingNoOpActionDescription Instance = new();

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(1);
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