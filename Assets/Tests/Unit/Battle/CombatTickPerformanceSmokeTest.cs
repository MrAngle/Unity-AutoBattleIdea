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

namespace MageFactory.Tests.Unit.Battle {
    public sealed class CombatTickPerformanceSmokeTest {
        private const int TargetRuntimeParticipants = 1_000;
        private const int InventoryWidth = 17;
        private const int InventoryHeight = 8;
        private const int ChainLengthIncludingEntryPoint = 5;
        private const int EntryPointTriggerIntervalTicks = 10_000;
        private const int WarmupTicks = 1;
        private const int MeasuredTicks = 3;
        private const double MaxAverageTickMilliseconds = 100.0;

        private static readonly ShapeArchetype PerformanceEntryPointShape = new(
            ShapeArchetypeId.SQUARE_1X1,
            "Performance Entry Point",
            ItemShape.singleCell());

        private static readonly ShapeArchetype PerformanceChainItemShape = new(
            ShapeArchetypeId.SQUARE_1X1,
            "Performance Chain Item",
            ItemShape.singleCell());

        // do sprwadzenia
        [Test]
        [Category("PerformanceSmoke")]
        public void should_tick_about_1000_items_and_1000_active_flows_within_smoke_budget() {
            var previousLogState = Debug.unityLogger.logEnabled;

            try {
                Debug.unityLogger.logEnabled = false;

                ICombatContext combatContext = BattleScenarioTestHarness.create()
                    .withFlowSettings(maxStepsPerSlice: 1)
                    .createContext(createCombatCommandsWithIsolatedEntryPoints(TargetRuntimeParticipants));

                createInitialFlows(combatContext);
                var session = BattleSessionTestFixtures.basic(combatContext);

                Assert.AreEqual(
                    TargetRuntimeParticipants,
                    combatContext.getCombatCapabilities().query().getActiveFlowCount());
                Assert.AreEqual(
                    TargetRuntimeParticipants,
                    combatContext.getCombatCapabilities().query().getCreatedFlowCount());

                session.tickMany(new ManualBattleLoop(), WarmupTicks);

                int activeFlowsBeforeMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getActiveFlowCount();
                int createdFlowsBeforeMeasurement = combatContext
                    .getCombatCapabilities()
                    .query()
                    .getCreatedFlowCount();

                Assert.AreEqual(TargetRuntimeParticipants, activeFlowsBeforeMeasurement);
                Assert.AreEqual(TargetRuntimeParticipants, createdFlowsBeforeMeasurement);

                var stopwatch = Stopwatch.StartNew();
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
                    $"Combat tick performance smoke: {activeFlowsBeforeMeasurement} active flows, " +
                    $"{TargetRuntimeParticipants} tickable items target, " +
                    $"{averageTickMilliseconds:0.###} ms/tick average over {MeasuredTicks} measured ticks.");

                Assert.AreEqual(TargetRuntimeParticipants, activeFlowsAfterMeasurement);
                Assert.AreEqual(TargetRuntimeParticipants, createdFlowsAfterMeasurement);
                Assert.LessOrEqual(
                    averageTickMilliseconds,
                    MaxAverageTickMilliseconds,
                    "This is only a smoke threshold for catastrophic combat tick regressions. " +
                    "If it fails because of legitimate gameplay growth, raise the threshold deliberately " +
                    "and keep the new budget documented.");
            }
            finally {
                Debug.unityLogger.logEnabled = previousLogState;
            }
        }

        private static void createInitialFlows(ICombatContext combatContext) {
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
                }
            }
        }

        private static CreateCombatCharacterCommand[] createCombatCommandsWithIsolatedEntryPoints(
            int targetEntryPointCount) {
            var commands = new List<CreateCombatCharacterCommand>();
            var remainingEntryPoints = targetEntryPointCount;
            var attackerIndex = 0;

            while (remainingEntryPoints > 0) {
                int entryPointCount = Math.Min(
                    remainingEntryPoints,
                    getMaxIsolatedEntryPointsPerCharacter());

                commands.Add(new CreateCombatCharacterCommand(
                    $"Performance Attacker {attackerIndex}",
                    1_000_000_000,
                    Team.TeamA,
                    createIsolatedEntryPointCommands(entryPointCount)));

                remainingEntryPoints -= entryPointCount;
                attackerIndex++;
            }

            commands.Add(new CreateCombatCharacterCommand(
                "Performance Defender",
                1_000_000_000,
                Team.TeamB,
                Array.Empty<EquipItemCommand>()));

            return commands.ToArray();
        }

        private static EquipItemCommand[] createIsolatedEntryPointCommands(int count) {
            var commands = new List<EquipItemCommand>(count * ChainLengthIncludingEntryPoint);
            var commandIndex = 0;

            for (var y = 0; y < InventoryHeight && commandIndex < count; y += 2) {
                for (var x = 0;
                     x + ChainLengthIncludingEntryPoint <= InventoryWidth && commandIndex < count;
                     x += ChainLengthIncludingEntryPoint + 1) {
                    commands.Add(new EquipItemCommand(
                        new PerformanceEntryPointDefinition(),
                        new Vector2Int(x, y)));

                    for (var chainIndex = 1; chainIndex < ChainLengthIncludingEntryPoint; chainIndex++) {
                        commands.Add(new EquipItemCommand(
                            new PerformanceChainItemDefinition(),
                            new Vector2Int(x + chainIndex, y)));
                    }

                    commandIndex++;
                }
            }

            if (commandIndex != count) {
                throw new InvalidOperationException(
                    $"Could only place {commandIndex} isolated entry points, requested {count}.");
            }

            return commands.ToArray();
        }

        private static int getMaxIsolatedEntryPointsPerCharacter() {
            var chainsPerAvailableRow = 0;
            for (var x = 0;
                 x + ChainLengthIncludingEntryPoint <= InventoryWidth;
                 x += ChainLengthIncludingEntryPoint + 1) {
                chainsPerAvailableRow++;
            }

            return chainsPerAvailableRow * ((InventoryHeight + 1) / 2);
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

        private sealed class PerformanceChainItemDefinition : IItemDefinition {
            public ShapeArchetype getShape() {
                return PerformanceChainItemShape;
            }

            public IActionDescription getActionDescription() {
                return NoOpActionDescription.Instance;
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