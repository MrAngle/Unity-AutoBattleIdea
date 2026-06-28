using System.Collections.Generic;
using System.Linq;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class DamagePacketProcessingTest {
        [Test]
        public void should_process_attack_damage_as_tick_delayed_layer_packet() {
            const int attackPower = 100;
            const int ticksPerLayer = 2;
            var listener = new DamagePacketLayerListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .withDamagePacketTicksPerStage(ticksPerLayer)
                .withDamagePacketLayerProcessedListener(listener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(attackPower),
                            new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long defenderInitialHp = defender.query().getCharacterInfo().getCurrentHp();

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(defenderInitialHp, defender.query().getCharacterInfo().getCurrentHp());

            combatContext.combatTick(CombatTicks.of(ticksPerLayer));

            Assert.AreEqual(defenderInitialHp, defender.query().getCharacterInfo().getCurrentHp());
            Assert.That(listener.layers, Is.EqualTo(new[] {
                DamagePacketLayer.Travel,
                DamagePacketLayer.IncomingHit
            }));
            Assert.That(listener.latest.completesPacket, Is.True);

            combatContext.combatTick(CombatTicks.of(ticksPerLayer));

            Assert.AreEqual(defenderInitialHp, defender.query().getCharacterInfo().getCurrentHp());
            Assert.That(listener.layers.Last(), Is.EqualTo(DamagePacketLayer.Stability));
            Assert.AreEqual(50, listener.latest.outgoingDamage);

            combatContext.combatTick(CombatTicks.of(ticksPerLayer));

            Assert.AreEqual(defenderInitialHp, defender.query().getCharacterInfo().getCurrentHp());
            Assert.That(listener.layers.Last(), Is.EqualTo(DamagePacketLayer.Guard));

            combatContext.combatTick(CombatTicks.of(ticksPerLayer));

            Assert.AreEqual(defenderInitialHp - 50, defender.query().getCharacterInfo().getCurrentHp());
            Assert.That(listener.layers.Last(), Is.EqualTo(DamagePacketLayer.Hp));
            Assert.That(listener.latest.completesPacket, Is.True);
        }

        [Test]
        public void should_process_pending_damage_packets_without_throwing_after_target_dies() {
            const int attackPower = 10;
            const int ticksPerLayer = 1;
            var deathListener = new CountingDeathEventListener();

            IItemDefinition firstEntry = new AttackEntryPointDefinition(attackPower);
            IItemDefinition secondEntry = new AttackEntryPointDefinition(attackPower);

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .withDamagePacketTicksPerStage(ticksPerLayer)
                .withCharacterDeathListener(deathListener)
                .createContext(
                    new CreateCombatCharacterCommand(
                        "Attacker",
                        1_000_000,
                        Team.TeamA,
                        new GridDimensions(17, 8),
                        new[] {
                            new EquipItemCommand(firstEntry, new Vector2Int(0, 0)),
                            new EquipItemCommand(secondEntry, new Vector2Int(2, 0))
                        }),
                    new CreateCombatCharacterCommand(
                        "Defender",
                        1,
                        Team.TeamB,
                        new GridDimensions(17, 8),
                        new EquipItemCommand[] { }));

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(2, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.DoesNotThrow(() => TestHelpers.tickCombatContext(combatContext, 6));
            Assert.AreEqual(0, getCharacterByTeam(combatContext, Team.TeamB).query().getCharacterInfo().getCurrentHp());
            Assert.AreEqual(1, deathListener.getDeathEventCount());
        }

        [Test]
        public void should_process_all_damage_packets_after_many_defensive_inputs() {
            const int packetCount = 50;
            const int attackPower = 10;
            const int ticksPerLayer = 1;
            const int inventoryColumns = 10;
            const int inventoryRows = 5;
            var listener = new DamagePacketLayerListener();
            var attackerItems = new List<EquipItemCommand>(packetCount);
            var defenderItems = new List<EquipItemCommand>(packetCount);

            for (int i = 0; i < packetCount; i++) {
                Vector2Int origin = new Vector2Int((i % inventoryColumns) * 2, (i / inventoryColumns) * 2);
                attackerItems.Add(new EquipItemCommand(new AttackEntryPointDefinition(attackPower), origin));
                defenderItems.Add(new EquipItemCommand(new DefensiveEntryPointDefinition(), origin));
            }

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withFlowSettings(maxStepsPerSlice: 250)
                .withInstantFlowItemCastTime()
                .withDamagePacketTicksPerStage(ticksPerLayer)
                .withDamagePacketLayerProcessedListener(listener)
                .createContext(
                    new CreateCombatCharacterCommand(
                        "Attacker",
                        1_000_000,
                        Team.TeamA,
                        new GridDimensions(inventoryColumns * 2, inventoryRows * 2),
                        attackerItems),
                    new CreateCombatCharacterCommand(
                        "Defender",
                        1_000_000,
                        Team.TeamB,
                        new GridDimensions(inventoryColumns * 2, inventoryRows * 2),
                        defenderItems));

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long initialHp = defender.query().getCharacterInfo().getCurrentHp();

            foreach (IGridItemPlaced item in attacker.query().getInventoryAggregate().getPlacedSnapshot()) {
                createFlow(combatContext, attacker, item);
            }

            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());
            TestHelpers.tickCombatContext(combatContext, ticksPerLayer);

            Assert.AreEqual(0, combatContext.getCombatCapabilities().query().getActiveDamagePacketCount());
            Assert.AreEqual(packetCount, defender.query().getActiveFlowCount());

            defender.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());
            TestHelpers.tickCombatContext(combatContext, ticksPerLayer * 3);

            long expectedDamage = TestHelpers.getDamageAfterDefaultStability(
                Enumerable.Repeat((long)attackPower, packetCount));

            Assert.AreEqual(0, combatContext.getCombatCapabilities().query().getActiveDamagePacketCount());
            Assert.AreEqual(0, defender.query().getActiveFlowCount());
            Assert.AreEqual(initialHp - expectedDamage, defender.query().getCharacterInfo().getCurrentHp());
            Assert.AreEqual(packetCount, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(packetCount, listener.count(DamagePacketLayer.Travel));
            Assert.AreEqual(packetCount, listener.count(DamagePacketLayer.IncomingHit));
            Assert.AreEqual(packetCount, listener.count(DamagePacketLayer.Stability));
            Assert.AreEqual(packetCount, listener.count(DamagePacketLayer.Guard));
            Assert.AreEqual(packetCount, listener.count(DamagePacketLayer.Hp));
            Assert.AreEqual(packetCount, listener.countCompleted(DamagePacketLayer.IncomingHit));
            Assert.AreEqual(packetCount, listener.countCompleted(DamagePacketLayer.Hp));
        }

        private static ICombatCharacterFacade getCharacterByTeam(ICombatContext combatContext, Team team) {
            return combatContext.getAllCharacters()
                .First(character => character.query().getCharacterInfo().getTeam() == team);
        }

        private static IGridItemPlaced getPlacedItemAt(ICombatCharacterFacade character, Vector2Int origin) {
            return character.query()
                .getInventoryAggregate()
                .getPlacedSnapshot()
                .First(item => item.getOrigin() == origin);
        }

        private static void createFlow(
            ICombatContext combatContext,
            ICombatCharacterFacade character,
            IGridItemPlaced entryPointItem) {
            combatContext.getCombatCapabilities()
                .command()
                .dispatch(new CreateFlowCombatCommand(
                    character.query().getCharacterInfo().getCharacterId(),
                    entryPointItem.getId()));
        }

        private sealed class AttackEntryPointDefinition : IEntryPointDefinition {
            private readonly int attackPower;

            internal AttackEntryPointDefinition(int attackPower) {
                this.attackPower = attackPower;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new PowerActionDescription(attackPower);
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
                return CombatTicks.of(10_000);
            }
        }

        private sealed class DefensiveEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new PowerActionDescription(0);
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

        private sealed class PowerActionDescription : IActionDescription {
            private readonly int power;

            internal PowerActionDescription(int power) {
                this.power = power;
            }

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(1);
            }

            public IOperations getEffectsDescriptor() {
                return new TestOperations(new AddPower(DamageRole.ATTACK, new PowerAmount(power)));
            }
        }

        private sealed class TestOperations : IOperations {
            private readonly IOperation[] effects;

            internal TestOperations(params IOperation[] effects) {
                this.effects = effects;
            }

            public IReadOnlyList<IOperation> getEffects() {
                return effects;
            }
        }

        private sealed class DamagePacketLayerListener : IDamagePacketLayerProcessedEventListener {
            internal readonly List<DamagePacketLayer> layers = new();
            private readonly List<DamagePacketLayerProcessedDtoEvent> events = new();
            internal DamagePacketLayerProcessedDtoEvent latest;

            public void onEvent(in DamagePacketLayerProcessedDtoEvent ev) {
                layers.Add(ev.layer);
                events.Add(ev);
                latest = ev;
            }

            internal int count(DamagePacketLayer layer) {
                return layers.Count(candidate => candidate == layer);
            }

            internal int countCompleted(DamagePacketLayer layer) {
                return events.Count(candidate => candidate.layer == layer && candidate.completesPacket);
            }
        }

        private sealed class CountingDeathEventListener : ICharacterDeathEventListener {
            private int deathEventCount;

            public void onEvent(in CharacterDeathDtoEvent ev) {
                deathEventCount++;
            }

            internal int getDeathEventCount() {
                return deathEventCount;
            }
        }
    }
}