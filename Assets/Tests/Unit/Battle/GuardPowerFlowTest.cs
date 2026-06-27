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
    public sealed class GuardPowerFlowTest {
        [Test]
        public void should_create_prepared_guard_from_flow_guard_power_output() {
            const int guardPower = 120;

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new GuardEntryPointDefinition(guardPower),
                            new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade guardOwner = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade enemy = getCharacterByTeam(combatContext, Team.TeamB);
            long enemyInitialHp = enemy.query().getCharacterInfo().getCurrentHp();

            createFlow(combatContext, guardOwner, getPlacedItemAt(guardOwner, new Vector2Int(0, 0)));
            guardOwner.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, guardOwner.query().getPreparedGuardCount());
            Assert.AreEqual(guardPower, guardOwner.query().getTotalPreparedGuardPower());
            Assert.AreEqual(enemyInitialHp, enemy.query().getCharacterInfo().getCurrentHp());
            Assert.AreEqual(0, combatContext.getCombatCapabilities().query()
                .getCombatEventCount(CombatEventType.INCOMING_ATTACK_DAMAGE));
        }

        [Test]
        public void should_publish_guard_created_event_from_final_flow_item() {
            const int guardPower = 5;
            var listener = new FlowGuardCreatedListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .withFlowGuardCreatedListener(listener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new GuardEntryPointDefinition(0),
                            new Vector2Int(0, 0)),
                        new EquipItemCommand(
                            new GuardItemDefinition(guardPower),
                            new Vector2Int(1, 0))
                    });

            ICombatCharacterFacade guardOwner = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced finalGuardItem = getPlacedItemAt(guardOwner, new Vector2Int(1, 0));

            createFlow(combatContext, guardOwner, getPlacedItemAt(guardOwner, new Vector2Int(0, 0)));
            guardOwner.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, listener.callCount);
            Assert.AreEqual(guardOwner.query().getCharacterInfo().getCharacterId(), listener.latest.characterId);
            Assert.AreEqual(guardPower, listener.latest.guardPower);
            Assert.That(listener.latest.hasSourceProcessingSlot(), Is.True);
            Assert.AreEqual(finalGuardItem.getId(), listener.latest.sourceProcessingSlot.getItemId());
        }

        [Test]
        public void should_reduce_incoming_damage_with_prepared_guard_and_strain_guard_power() {
            const int guardPower = 120;
            const int incomingDamage = 120;

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(incomingDamage),
                            new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long defenderInitialHp = defender.query().getCharacterInfo().getCurrentHp();
            long damageAfterStability = TestHelpers.getDamageAfterDefaultStability(incomingDamage);
            long expectedBlockedDamage =
                GuardMitigationCalculator.calculateBlockedDamage(guardPower, damageAfterStability);

            addGuardOrFail(defender, guardPower);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(
                defenderInitialHp - (damageAfterStability - expectedBlockedDamage),
                defender.query().getCharacterInfo().getCurrentHp());
            Assert.AreEqual(1, defender.query().getPreparedGuardCount());
            Assert.AreEqual(guardPower - expectedBlockedDamage, defender.query().getTotalPreparedGuardPower());
        }

        [Test]
        public void should_publish_guard_absorb_event_and_destroy_empty_guard() {
            const int guardPower = 3;
            const int incomingDamage = 10;
            var listener = new GuardAbsorbedDamageListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .withGuardAbsorbedDamageListener(listener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(incomingDamage),
                            new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long defenderInitialHp = defender.query().getCharacterInfo().getCurrentHp();
            long damageAfterStability = TestHelpers.getDamageAfterDefaultStability(incomingDamage);
            long expectedBlockedDamage =
                GuardMitigationCalculator.calculateBlockedDamage(guardPower, damageAfterStability);

            addGuardOrFail(defender, guardPower);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, listener.callCount);
            Assert.AreEqual(defender.query().getCharacterInfo().getCharacterId(), listener.latest.characterId);
            Assert.AreEqual(damageAfterStability, listener.latest.incomingDamage);
            Assert.AreEqual(expectedBlockedDamage, listener.latest.blockedDamage);
            Assert.AreEqual(damageAfterStability - expectedBlockedDamage, listener.latest.remainingDamage);
            Assert.AreEqual(1, listener.latest.destroyedGuardCount);
            Assert.AreEqual(0, listener.latest.remainingGuardPower);
            Assert.AreEqual(0, defender.query().getPreparedGuardCount());
            Assert.AreEqual(defenderInitialHp - listener.latest.remainingDamage,
                defender.query().getCharacterInfo().getCurrentHp());
        }

        [Test]
        public void should_publish_attack_created_event_from_final_flow_item_to_target() {
            const int attackPower = 10;
            var listener = new FlowAttackCreatedListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .withFlowAttackCreatedListener(listener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(0),
                            new Vector2Int(0, 0)),
                        new EquipItemCommand(
                            new AttackItemDefinition(attackPower),
                            new Vector2Int(1, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            IGridItemPlaced finalAttackItem = getPlacedItemAt(attacker, new Vector2Int(1, 0));

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, listener.callCount);
            Assert.AreEqual(attacker.query().getCharacterInfo().getCharacterId(), listener.latest.sourceCharacterId);
            Assert.AreEqual(defender.query().getCharacterInfo().getCharacterId(), listener.latest.targetCharacterId);
            Assert.AreEqual(attackPower, listener.latest.attackPower);
            Assert.That(listener.latest.hasSourceProcessingSlot(), Is.True);
            Assert.AreEqual(finalAttackItem.getId(), listener.latest.sourceProcessingSlot.getItemId());
        }

        [Test]
        public void should_keep_guard_portion_count_readable_by_replacing_oldest_overflow() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(Array.Empty<EquipItemCommand>());

            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);

            for (int i = 0; i < 35; i++) {
                addGuardOrFail(defender, 10);
            }

            Assert.AreEqual(30, defender.query().getPreparedGuardCount());
            Assert.AreEqual(300, defender.query().getTotalPreparedGuardPower());
        }

        [Test]
        public void should_report_oldest_guard_replacement_when_guard_queue_overflows() {
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(Array.Empty<EquipItemCommand>());

            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            PreparedGuardAddResult oldestGuardAddResult = addGuardOrFail(defender, 10);

            for (int i = 1; i < 30; i++) {
                addGuardOrFail(defender, 100 + i);
            }

            PreparedGuardAddResult overflowGuardAddResult = addGuardOrFail(defender, 999);

            Assert.That(overflowGuardAddResult.hasReplacedGuard(), Is.True);
            Assert.AreEqual(
                oldestGuardAddResult.getAddedGuardState().getGuardId(),
                overflowGuardAddResult.getReplacedGuardState().getGuardId());
            Assert.AreEqual(10, overflowGuardAddResult.getReplacedGuardState().getGuardPower().getPower());
            Assert.AreEqual(999, overflowGuardAddResult.getAddedGuardState().getGuardPower().getPower());
            Assert.AreEqual(30, defender.query().getPreparedGuardCount());
        }

        [Test]
        public void should_consume_prepared_guards_from_left_to_right() {
            const int firstGuardPower = 3;
            const int secondGuardPower = 120;
            const int incomingDamage = 10;

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(
                            new AttackEntryPointDefinition(incomingDamage),
                            new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long damageAfterStability = TestHelpers.getDamageAfterDefaultStability(incomingDamage);
            long expectedFirstGuardBlockedDamage =
                GuardMitigationCalculator.calculateBlockedDamage(firstGuardPower, damageAfterStability);
            long expectedRemainingAfterFirstGuard = damageAfterStability - expectedFirstGuardBlockedDamage;
            long expectedSecondGuardBlockedDamage =
                GuardMitigationCalculator.calculateBlockedDamage(secondGuardPower, expectedRemainingAfterFirstGuard);

            addGuardOrFail(defender, firstGuardPower);
            addGuardOrFail(defender, secondGuardPower);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, defender.query().getPreparedGuardCount());
            Assert.AreEqual(firstGuardPower, expectedFirstGuardBlockedDamage);
            Assert.AreEqual(
                secondGuardPower - expectedSecondGuardBlockedDamage,
                defender.query().getTotalPreparedGuardPower());
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

        private static PreparedGuardAddResult addGuardOrFail(
            ICombatCharacterFacade character,
            int guardPower) {
            bool added = character.command()
                .tryAddGuardPower(new GuardPower(guardPower), out PreparedGuardAddResult guardAddResult);
            Assert.That(added, Is.True);
            Assert.That(guardAddResult.hasAddedGuard(), Is.True);
            return guardAddResult;
        }

        private sealed class GuardEntryPointDefinition : IEntryPointDefinition {
            private readonly int guardPower;

            internal GuardEntryPointDefinition(int guardPower) {
                this.guardPower = guardPower;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new GuardPowerActionDescription(guardPower);
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

        private sealed class AttackEntryPointDefinition : IEntryPointDefinition {
            private readonly int attackPower;

            internal AttackEntryPointDefinition(int attackPower) {
                this.attackPower = attackPower;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new AttackActionDescription(attackPower);
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

        private sealed class GuardItemDefinition : IItemDefinition {
            private readonly int guardPower;

            internal GuardItemDefinition(int guardPower) {
                this.guardPower = guardPower;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new GuardPowerActionDescription(guardPower);
            }
        }

        private sealed class AttackItemDefinition : IItemDefinition {
            private readonly int attackPower;

            internal AttackItemDefinition(int attackPower) {
                this.attackPower = attackPower;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new AttackActionDescription(attackPower);
            }
        }

        private sealed class GuardPowerActionDescription : IActionDescription {
            private readonly int guardPower;

            internal GuardPowerActionDescription(int guardPower) {
                this.guardPower = guardPower;
            }

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(1);
            }

            public IOperations getEffectsDescriptor() {
                return new TestOperations(new AddGuardPower(new GuardPower(guardPower)));
            }
        }

        private sealed class AttackActionDescription : IActionDescription {
            private readonly int attackPower;

            internal AttackActionDescription(int attackPower) {
                this.attackPower = attackPower;
            }

            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(1);
            }

            public IOperations getEffectsDescriptor() {
                return new TestOperations(new AddPower(DamageRole.ATTACK, new PowerAmount(attackPower)));
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

        private sealed class GuardAbsorbedDamageListener : IGuardAbsorbedDamageEventListener {
            internal int callCount;
            internal CharacterGuardAbsorbedDamageDtoEvent latest;

            public void onEvent(in CharacterGuardAbsorbedDamageDtoEvent ev) {
                callCount++;
                latest = ev;
            }
        }

        private sealed class FlowGuardCreatedListener : IFlowGuardCreatedEventListener {
            internal int callCount;
            internal FlowGuardCreatedDtoEvent latest;

            public void onEvent(in FlowGuardCreatedDtoEvent ev) {
                callCount++;
                latest = ev;
            }
        }

        private sealed class FlowAttackCreatedListener : IFlowAttackCreatedEventListener {
            internal int callCount;
            internal FlowAttackCreatedDtoEvent latest;

            public void onEvent(in FlowAttackCreatedDtoEvent ev) {
                callCount++;
                latest = ev;
            }
        }
    }
}