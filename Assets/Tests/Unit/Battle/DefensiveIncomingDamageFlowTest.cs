using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Item.Catalog.Bases;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class DefensiveIncomingDamageFlowTest {
        private const int IncomingAttackDamage = 10;
        private const int DefensiveEntryDamageReduction = -3;
        private const int DefensiveShieldDamageReduction = -5;
        private const int DefenseEntryPointGemGuardPower = 2;
        private const int CatalogShieldGuardPower = 5;

        [Test]
        public void should_not_tick_event_triggered_defensive_entry_point_without_incoming_attack_damage_event() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    Array.Empty<EquipItemCommand>(),
                    new[] {
                        new EquipItemCommand(
                            new DefensiveEntryPointDefinition(DefensiveEntryDamageReduction),
                            new Vector2Int(0, 0)),
                        new EquipItemCommand(
                            new PowerChangeBattleItemDefinition(DefensiveShieldDamageReduction),
                            new Vector2Int(1, 0))
                    });

            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);

            defender.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(0, defender.query().getActiveFlowCount());
            Assert.AreEqual(0, defender.query().getCreatedFlowsInCurrentBattleCount());
            Assert.AreEqual(0, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_create_defensive_flow_from_incoming_attack_damage_event_before_hp_is_changed() {
            ICombatContext combatContext = createCustomDefenseContext();
            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long initialHp = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));

            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(initialHp, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(1, defender.query().getActiveFlowCount());
            Assert.AreEqual(1, defender.query().getCreatedFlowsInCurrentBattleCount());
            Assert.AreEqual(1, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(1, defender.query().getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(0, attacker.query().getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_apply_defensive_flow_result_without_republishing_incoming_attack_damage_event() {
            ICombatContext combatContext = createCustomDefenseContext();
            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long initialHp = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            defender.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            int expectedDamage =
                IncomingAttackDamage +
                DefensiveEntryDamageReduction +
                DefensiveShieldDamageReduction;
            long expectedDamageAfterStability = TestHelpers.getDamageAfterDefaultStability(expectedDamage);

            Assert.AreEqual(initialHp - expectedDamageAfterStability, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(0, defender.query().getActiveFlowCount());
            Assert.AreEqual(1, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(1, defender.query().getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_apply_incoming_attack_damage_directly_when_no_defensive_entry_point_exists() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(IncomingAttackDamage),
                            new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long initialHp = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));

            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(
                initialHp - TestHelpers.getDamageAfterDefaultStability(IncomingAttackDamage),
                TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(0, defender.query().getActiveFlowCount());
            Assert.AreEqual(0, defender.query().getCreatedFlowsInCurrentBattleCount());
            Assert.AreEqual(1, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(1, defender.query().getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(0, attacker.query().getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_apply_incoming_attack_damage_directly_when_defensive_entry_point_is_busy() {
            ICombatContext combatContext = createCustomDefenseContext();
            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long initialHp = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            createFlow(combatContext, defender, getPlacedItemAt(defender, new Vector2Int(0, 0)));
            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));

            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(
                initialHp - TestHelpers.getDamageAfterDefaultStability(IncomingAttackDamage),
                TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(1, defender.query().getActiveFlowCount());
            Assert.AreEqual(1, defender.query().getCreatedFlowsInCurrentBattleCount());
            Assert.AreEqual(1, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(1, defender.query().getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_use_next_available_defensive_entry_point_when_first_defensive_entry_point_is_busy() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withFlowSettings(maxStepsPerSlice: 4)
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(IncomingAttackDamage),
                            new Vector2Int(0, 0))
                    },
                    new[] {
                        new EquipItemCommand(
                            new DefensiveEntryPointDefinition(DefensiveEntryDamageReduction),
                            new Vector2Int(0, 0)),
                        new EquipItemCommand(
                            new DefensiveEntryPointDefinition(DefensiveEntryDamageReduction),
                            new Vector2Int(2, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            IGridItemPlaced firstDefenseEntryPoint = getPlacedItemAt(defender, new Vector2Int(0, 0));
            IGridItemPlaced secondDefenseEntryPoint = getPlacedItemAt(defender, new Vector2Int(2, 0));
            long initialHp = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            createFlow(combatContext, defender, firstDefenseEntryPoint);
            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));

            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(initialHp, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(2, defender.query().getActiveFlowCount());
            Assert.AreEqual(2, defender.query().getCreatedFlowsInCurrentBattleCount());
            Assert.AreEqual(1, defender.query().getActiveFlowCountOnItem(firstDefenseEntryPoint.getId()));
            Assert.AreEqual(1, defender.query().getActiveFlowCountOnItem(secondDefenseEntryPoint.getId()));
            Assert.AreEqual(1, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_rotate_successful_defensive_entry_point_selection_round_robin() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withFlowSettings(maxStepsPerSlice: 4)
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(IncomingAttackDamage),
                            new Vector2Int(0, 0))
                    },
                    new[] {
                        new EquipItemCommand(
                            new DefensiveEntryPointDefinition(DefensiveEntryDamageReduction),
                            new Vector2Int(0, 0)),
                        new EquipItemCommand(
                            new DefensiveEntryPointDefinition(DefensiveEntryDamageReduction),
                            new Vector2Int(2, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            IGridItemPlaced attackEntryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 0));
            IGridItemPlaced firstDefenseEntryPoint = getPlacedItemAt(defender, new Vector2Int(0, 0));
            IGridItemPlaced secondDefenseEntryPoint = getPlacedItemAt(defender, new Vector2Int(2, 0));

            createFlow(combatContext, attacker, attackEntryPoint);
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, defender.query().getActiveFlowCountOnItem(firstDefenseEntryPoint.getId()));
            Assert.AreEqual(0, defender.query().getActiveFlowCountOnItem(secondDefenseEntryPoint.getId()));

            defender.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());
            createFlow(combatContext, attacker, attackEntryPoint);
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(0, defender.query().getActiveFlowCountOnItem(firstDefenseEntryPoint.getId()));
            Assert.AreEqual(1, defender.query().getActiveFlowCountOnItem(secondDefenseEntryPoint.getId()));
            Assert.AreEqual(2, defender.query().getCreatedFlowsInCurrentBattleCount());
            Assert.AreEqual(2, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_process_catalog_defensive_entry_point_and_shield_against_incoming_attack_damage() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withFlowSettings(maxStepsPerSlice: 4)
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(IncomingAttackDamage),
                            new Vector2Int(0, 0))
                    },
                    new[] {
                        new EquipItemCommand(new DefenseEntryPointGem(), new Vector2Int(0, 0)),
                        new EquipItemCommand(new Shield(), new Vector2Int(1, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long initialHp = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());
            defender.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            int generatedGuardPower = DefenseEntryPointGemGuardPower + CatalogShieldGuardPower;
            long damageAfterStability = TestHelpers.getDamageAfterDefaultStability(IncomingAttackDamage);
            long expectedBlockedDamage = GuardMitigationCalculator.calculateBlockedDamage(
                generatedGuardPower,
                damageAfterStability);
            long expectedDamage = damageAfterStability - expectedBlockedDamage;

            Assert.AreEqual(initialHp - expectedDamage, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(1, defender.query().getPreparedGuardCount());
            Assert.AreEqual(generatedGuardPower - expectedBlockedDamage, defender.query().getTotalPreparedGuardPower());
            Assert.AreEqual(0, defender.query().getActiveFlowCount());
            Assert.AreEqual(1, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
            Assert.AreEqual(1, defender.query().getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        private static ICombatContext createCustomDefenseContext() {
            return BattleScenarioTestHarness.create()
                .withFlowSettings(maxStepsPerSlice: 4)
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(IncomingAttackDamage),
                            new Vector2Int(0, 0))
                    },
                    new[] {
                        new EquipItemCommand(
                            new DefensiveEntryPointDefinition(DefensiveEntryDamageReduction),
                            new Vector2Int(0, 0)),
                        new EquipItemCommand(
                            new PowerChangeBattleItemDefinition(DefensiveShieldDamageReduction),
                            new Vector2Int(1, 0))
                    });
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
                return new PowerChangeActionDescription(attackPower);
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
            private readonly int damageReduction;

            internal DefensiveEntryPointDefinition(int damageReduction) {
                this.damageReduction = damageReduction;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new PowerChangeActionDescription(damageReduction);
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

        private sealed class PowerChangeBattleItemDefinition : IItemDefinition {
            private readonly int powerDelta;

            internal PowerChangeBattleItemDefinition(int powerDelta) {
                this.powerDelta = powerDelta;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new PowerChangeActionDescription(powerDelta);
            }
        }

        private sealed class PowerChangeActionDescription : IActionDescription {
            private readonly int powerDelta;

            internal PowerChangeActionDescription(int powerDelta) {
                this.powerDelta = powerDelta;
            }

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(1);
            }

            public IOperations getEffectsDescriptor() {
                return new PowerChangeOperations(powerDelta);
            }
        }

        private sealed class PowerChangeOperations : IOperations {
            private readonly IOperation[] effects;

            internal PowerChangeOperations(int powerDelta) {
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