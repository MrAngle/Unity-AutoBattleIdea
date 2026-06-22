using System.Collections.Generic;
using System.Linq;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.Item.Catalog.Bases;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class FlowPortOutputTest {
        [Test]
        public void port_aware_flow_should_commit_accumulated_power_on_output_port() {
            var outputListener = new FlowOutputReachedListener();
            var attackListener = new FlowAttackCreatedListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withFlowOutputReachedListener(outputListener)
                .withFlowAttackCreatedListener(attackListener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(new PulseInputPort(), new Vector2Int(0, 0)),
                        new EquipItemCommand(new BasicOutputPort(), new Vector2Int(1, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced outputItem = getPlacedItemAt(attacker, new Vector2Int(1, 0));

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, outputListener.callCount);
            Assert.AreEqual(2, outputListener.latest.attackPower);
            Assert.AreEqual(outputItem.getId(), outputListener.latest.outputProcessingSlot.getItemId());
            Assert.AreEqual(1, attackListener.callCount);
            Assert.AreEqual(outputItem.getId(), attackListener.latest.sourceProcessingSlot.getItemId());
        }

        [Test]
        public void port_aware_flow_without_output_should_be_discarded_and_reported() {
            var noOutputListener = new FlowNoOutputListener();
            var attackListener = new FlowAttackCreatedListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withFlowNoOutputListener(noOutputListener)
                .withFlowAttackCreatedListener(attackListener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(new PulseInputPort(), new Vector2Int(0, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            ICombatCharacterFacade defender = getCharacterByTeam(combatContext, Team.TeamB);
            long defenderInitialHp = defender.query().getCharacterInfo().getCurrentHp();

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(1, noOutputListener.callCount);
            Assert.That(noOutputListener.latest.wasCommittedByLegacyRule, Is.False);
            Assert.AreEqual(2, noOutputListener.latest.attackPower);
            Assert.AreEqual(0, attackListener.callCount);
            Assert.AreEqual(defenderInitialHp, defender.query().getCharacterInfo().getCurrentHp());
        }

        [Test]
        public void output_port_should_not_cast_or_execute_its_own_effects() {
            var outputListener = new FlowOutputReachedListener();
            var attackListener = new FlowAttackCreatedListener();

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withFlowOutputReachedListener(outputListener)
                .withFlowAttackCreatedListener(attackListener)
                .create1V1WithEnormousHp(
                    new[] {
                        new EquipItemCommand(new PulseInputPort(), new Vector2Int(0, 0)),
                        new EquipItemCommand(new DangerousOutputPortDefinition(), new Vector2Int(1, 0))
                    });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);

            createFlow(combatContext, attacker, getPlacedItemAt(attacker, new Vector2Int(0, 0)));
            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(0, attacker.query().getActiveFlowCount());
            Assert.AreEqual(1, outputListener.callCount);
            Assert.AreEqual(2, outputListener.latest.attackPower);
            Assert.AreEqual(1, attackListener.callCount);
            Assert.AreEqual(2, attackListener.latest.attackPower);
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

        private sealed class DangerousOutputPortDefinition : IItemDefinition, IFlowPortDefinition {
            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new DangerousOutputActionDescription();
            }

            public FlowPortKind getFlowPortKind() {
                return FlowPortKind.Output;
            }

            public string getFlowPortName() {
                return "OUT";
            }

            public string getFlowPortDescription() {
                return "Test output that must not cast or execute effects.";
            }
        }

        private sealed class DangerousOutputActionDescription : IActionDescription {
            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(99);
            }

            public IOperations getEffectsDescriptor() {
                return new TestOperations(new AddPower(DamageRole.ATTACK, new PowerAmount(999)));
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

        private sealed class FlowOutputReachedListener : IFlowOutputReachedEventListener {
            internal int callCount;
            internal FlowOutputReachedDtoEvent latest;

            public void onEvent(in FlowOutputReachedDtoEvent ev) {
                callCount++;
                latest = ev;
            }
        }

        private sealed class FlowNoOutputListener : IFlowNoOutputEventListener {
            internal int callCount;
            internal FlowNoOutputDtoEvent latest;

            public void onEvent(in FlowNoOutputDtoEvent ev) {
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