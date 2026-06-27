using System;
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
    public sealed class StabilityFlowTest {
        [Test]
        public void should_start_each_combat_character_with_baseline_stability() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(new EquipItemCommand[] { });

            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);

            Assert.AreEqual(StabilityMitigationCalculator.ReferenceStability, defender.query().getCurrentStability());
            Assert.AreEqual(StabilityMitigationCalculator.ReferenceStability, defender.query().getBaselineStability());
        }

        [Test]
        public void should_apply_stability_before_hp_and_strain_it_by_incoming_damage() {
            const int incomingDamage = 1000;
            var listener = new StabilityAbsorbedDamageListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .withStabilityAbsorbedDamageListener(listener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(incomingDamage),
                            new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long defenderInitialHp = defender.query().getCharacterInfo().getCurrentHp();
            long expectedDamage = StabilityMitigationCalculator.calculateDamageAfterStability(
                StabilityMitigationCalculator.ReferenceStability,
                incomingDamage);
            long expectedStrain = StabilityMitigationCalculator.calculateDefaultStrain(incomingDamage);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(defenderInitialHp - expectedDamage, defender.query().getCharacterInfo().getCurrentHp());
            Assert.AreEqual(
                Math.Max(0, StabilityMitigationCalculator.ReferenceStability - expectedStrain),
                defender.query().getCurrentStability());
            Assert.AreEqual(1, listener.callCount);
            Assert.AreEqual(incomingDamage, listener.latest.incomingDamage);
            Assert.AreEqual(incomingDamage - expectedDamage, listener.latest.reducedDamage);
            Assert.AreEqual(expectedDamage, listener.latest.remainingDamage);
            Assert.AreEqual(expectedStrain, listener.latest.stabilityStrain);
        }

        [Test]
        public void should_create_stability_from_flow_output_and_publish_final_item_event() {
            const int stabilityPower = 2;
            var listener = new FlowStabilityCreatedListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .withFlowStabilityCreatedListener(listener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new StabilityEntryPointDefinition(0),
                            new Vector2Int(0, 0)),
                        new EquipItemCommand(
                            new StabilityItemDefinition(stabilityPower),
                            new Vector2Int(1, 0))
                    });

            ICombatCharacterFacade owner = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced finalStabilityItem = getPlacedItemAt(owner, new Vector2Int(1, 0));

            createFlow(combatContext, owner, getPlacedItemAt(owner, new Vector2Int(0, 0)));
            owner.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(StabilityMitigationCalculator.ReferenceStability + stabilityPower,
                owner.query().getCurrentStability());
            Assert.AreEqual(1, listener.callCount);
            Assert.AreEqual(owner.query().getCharacterInfo().getCharacterId(), listener.latest.characterId);
            Assert.AreEqual(stabilityPower, listener.latest.stabilityPower);
            Assert.AreEqual(StabilityMitigationCalculator.ReferenceStability, listener.latest.stabilityBefore);
            Assert.AreEqual(
                StabilityMitigationCalculator.ReferenceStability + stabilityPower,
                listener.latest.stabilityAfter);
            Assert.That(listener.latest.hasSourceProcessingSlot(), Is.True);
            Assert.AreEqual(finalStabilityItem.getId(), listener.latest.sourceProcessingSlot.getItemId());
        }

        [Test]
        public void should_use_baseline_stability_as_reduction_reference() {
            const int incomingDamage = 1000;

            long earlyDamage = StabilityMitigationCalculator.calculateDamageAfterStability(
                100,
                100,
                incomingDamage);
            long lateDamage = StabilityMitigationCalculator.calculateDamageAfterStability(
                1000,
                1000,
                incomingDamage);

            Assert.AreEqual(500, earlyDamage);
            Assert.AreEqual(earlyDamage, lateDamage);
        }

        [Test]
        public void should_decay_stability_only_after_overbaseline_threshold() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(new EquipItemCommand[] { });

            ICombatCharacterFacade owner = getCharacterByTeam(combatContext, Team.TeamA);

            owner.command().tryAddStabilityPower(new StabilityPower(30), out _);
            owner.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(130, owner.query().getCurrentStability());

            owner.command().tryAddStabilityPower(new StabilityPower(70), out _);
            owner.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(195, owner.query().getCurrentStability());

            owner.command().tryAddStabilityPower(new StabilityPower(109), out _);
            owner.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(249, owner.query().getCurrentStability());
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

        private sealed class StabilityEntryPointDefinition : IEntryPointDefinition {
            private readonly int stabilityPower;

            internal StabilityEntryPointDefinition(int stabilityPower) {
                this.stabilityPower = stabilityPower;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new StabilityActionDescription(stabilityPower);
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

        private sealed class StabilityItemDefinition : IItemDefinition {
            private readonly int stabilityPower;

            internal StabilityItemDefinition(int stabilityPower) {
                this.stabilityPower = stabilityPower;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new StabilityActionDescription(stabilityPower);
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

        private sealed class StabilityActionDescription : IActionDescription {
            private readonly int stabilityPower;

            internal StabilityActionDescription(int stabilityPower) {
                this.stabilityPower = stabilityPower;
            }

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(1);
            }

            public IOperations getEffectsDescriptor() {
                return new TestOperations(new AddStabilityPower(new StabilityPower(stabilityPower)));
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

        private sealed class StabilityAbsorbedDamageListener : IStabilityAbsorbedDamageEventListener {
            internal int callCount;
            internal CharacterStabilityAbsorbedDamageDtoEvent latest;

            public void onEvent(in CharacterStabilityAbsorbedDamageDtoEvent ev) {
                callCount++;
                latest = ev;
            }
        }

        private sealed class FlowStabilityCreatedListener : IFlowStabilityCreatedEventListener {
            internal int callCount;
            internal FlowStabilityCreatedDtoEvent latest;

            public void onEvent(in FlowStabilityCreatedDtoEvent ev) {
                callCount++;
                latest = ev;
            }
        }
    }
}