using System.Collections.Generic;
using System.Linq;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.BattleManager;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Item.Catalog.Bases;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class DeterministicFlowScenarioTest {
        private const int EntryPointTriggerTicks = 20;
        private const int EntryPointCastTicks = 5;
        private const int SingleFlowResolutionTicksBeforeNextTrigger = 35;
        private const int CarryTestEntryCastTicks = 3;
        private const int CarryTestNextItemCastTicks = 7;
        private const int CarryTestEntryDamage = 2;
        private const int CarryTestNextItemDamage = 5;
        private const int CapacityTestEntryCastTicks = 1;
        private const int CapacityTestSharedItemCastTicks = 100;
        private const int CapacityTestEntryDamage = 1;
        private const int CapacityTestSharedItemDamage = 50;
        private const int ShapeCastEntryCastTicks = 1;
        private const int ShapeCastWideItemBaseCastTicks = 4;
        private const int ShapeCastEntryDamage = 2;
        private const int ShapeCastWideItemDamage = 9;
        private const int DeathRegressionDefenderHp = 1;

        [Test]
        public void should_expose_active_and_created_flow_counts() {
            // given
            IItemDefinition entry1 = new EntryPointGem();
            IItemDefinition entry2 = new EntryPointGem();
            IItemDefinition entry3 = new EntryPointGem();
            IItemDefinition entry4 = new EntryPointGem();

            EquipItemCommand[] attackerItems = {
                new(entry1, new Vector2Int(0, 0)),
                new(entry2, new Vector2Int(1, 0)),
                new(entry3, new Vector2Int(2, 0)),
                new(entry4, new Vector2Int(3, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var attackerId = combatContext.getAllCharacters()
                .First(character => character.query().getCharacterInfo().getTeam() == Team.TeamA)
                .query()
                .getCharacterInfo()
                .getCharacterId();

            // when
            session.tickMany(new ManualBattleLoop(), EntryPointTriggerTicks);

            // then
            Assert.AreEqual(4, combatContext.getCombatCapabilities().query().getActiveFlowCount());
            Assert.AreEqual(4,
                combatContext.getCombatCapabilities().query().getActiveFlowCountForCharacter(attackerId));
            Assert.AreEqual(4, combatContext.getCombatCapabilities().query().getCreatedFlowCount());
            Assert.AreEqual(4,
                combatContext.getCombatCapabilities().query().getCreatedFlowCountForCharacter(attackerId));
        }

        [Test]
        public void should_use_shape_rows_as_flow_processing_capacity() {
            // given
            IItemDefinition entry1 = new CapacityTestEntryPointDefinition();
            IItemDefinition entry2 = new CapacityTestEntryPointDefinition();
            IItemDefinition entry3 = new CapacityTestEntryPointDefinition();
            IItemDefinition sharedItem = new CapacityTestBattleItemDefinition(ShapeCatalog.Square2x2);

            EquipItemCommand[] attackerItems = {
                new(entry1, new Vector2Int(5, 4)),
                new(sharedItem, new Vector2Int(5, 5)),
                new(entry2, new Vector2Int(7, 5)),
                new(entry3, new Vector2Int(5, 7))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced firstEntryPoint = getPlacedItemAt(attacker, new Vector2Int(5, 4));
            IGridItemPlaced secondEntryPoint = getPlacedItemAt(attacker, new Vector2Int(7, 5));
            IGridItemPlaced thirdEntryPoint = getPlacedItemAt(attacker, new Vector2Int(5, 7));
            IGridItemPlaced sharedPlacedItem = getPlacedItemAt(attacker, new Vector2Int(5, 5));

            createFlow(combatContext, attacker, firstEntryPoint);
            createFlow(combatContext, attacker, secondEntryPoint);
            createFlow(combatContext, attacker, thirdEntryPoint);

            // when
            attacker.command().combatTick(
                CombatTicks.of(CapacityTestEntryCastTicks),
                combatContext.getCombatCapabilities());

            // then
            Assert.AreEqual(2, attacker.query().getActiveFlowCount());
            Assert.AreEqual(2, attacker.query().getActiveFlowCountOnItem(sharedPlacedItem.getId()));
        }

        [Test]
        public void should_not_create_second_flow_when_entry_point_processing_slot_is_busy() {
            // given
            IItemDefinition entry = new CapacityTestEntryPointDefinition();

            EquipItemCommand[] attackerItems = {
                new(entry, new Vector2Int(0, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced entryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 0));

            // when
            createFlow(combatContext, attacker, entryPoint);
            createFlow(combatContext, attacker, entryPoint);

            // then
            Assert.AreEqual(1, attacker.query().getActiveFlowCount());
            Assert.AreEqual(1, attacker.query().getCreatedFlowsInCurrentBattleCount());
            Assert.AreEqual(1, attacker.query().getActiveFlowCountOnItem(entryPoint.getId()));
        }

        [Test]
        public void should_assign_unique_domain_flow_ids_to_active_flows() {
            // given
            EquipItemCommand[] attackerItems = new EquipItemCommand[9];
            for (int i = 0; i < attackerItems.Length; i++) {
                attackerItems[i] = new EquipItemCommand(
                    new CapacityTestEntryPointDefinition(),
                    new Vector2Int(i * 2, 0));
            }

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);

            foreach (EquipItemCommand attackerItem in attackerItems) {
                IGridItemPlaced entryPoint = getPlacedItemAt(attacker, attackerItem.origin);
                createFlow(combatContext, attacker, entryPoint);
            }

            // when
            var collector = new TestActiveFlowStateCollector();
            attacker.query().collectActiveFlowStates(collector);

            // then
            IReadOnlyList<ActiveFlowState> flowStates = collector.getStates();
            Assert.AreEqual(9, flowStates.Count);

            var flowIds = new HashSet<Id<ActiveFlowId>>();
            for (int i = 0; i < flowStates.Count; i++) {
                flowIds.Add(flowStates[i].getFlowId());
            }

            Assert.AreEqual(flowStates.Count, flowIds.Count);
        }

        [Test]
        public void should_route_to_available_neighbor_when_another_neighbor_is_full() {
            // given
            IItemDefinition firstEntry = new CapacityTestEntryPointDefinition();
            IItemDefinition secondEntry = new CapacityTestEntryPointDefinition();
            IItemDefinition fullItem = new CapacityTestBattleItemDefinition();
            IItemDefinition availableItem = new CapacityTestBattleItemDefinition();

            EquipItemCommand[] attackerItems = {
                new(firstEntry, new Vector2Int(0, 1)),
                new(fullItem, new Vector2Int(1, 1)),
                new(secondEntry, new Vector2Int(1, 0)),
                new(availableItem, new Vector2Int(2, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced firstEntryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 1));
            IGridItemPlaced secondEntryPoint = getPlacedItemAt(attacker, new Vector2Int(1, 0));
            IGridItemPlaced fullPlacedItem = getPlacedItemAt(attacker, new Vector2Int(1, 1));
            IGridItemPlaced availablePlacedItem = getPlacedItemAt(attacker, new Vector2Int(2, 0));

            createFlow(combatContext, attacker, firstEntryPoint);
            attacker.command().combatTick(
                CombatTicks.of(CapacityTestEntryCastTicks),
                combatContext.getCombatCapabilities());

            // when
            createFlow(combatContext, attacker, secondEntryPoint);
            attacker.command().combatTick(
                CombatTicks.of(CapacityTestEntryCastTicks),
                combatContext.getCombatCapabilities());

            // then
            Assert.AreEqual(2, attacker.query().getActiveFlowCount());
            Assert.AreEqual(1, attacker.query().getActiveFlowCountOnItem(fullPlacedItem.getId()));
            Assert.AreEqual(1, attacker.query().getActiveFlowCountOnItem(availablePlacedItem.getId()));
        }

        [Test]
        public void should_multiply_item_cast_time_by_processing_row_cell_count() {
            // given
            IItemDefinition entry = new ShapeCastEntryPointDefinition();
            IItemDefinition wideItem = new ShapeCastWideBattleItemDefinition();

            EquipItemCommand[] attackerItems = {
                new(entry, new Vector2Int(0, 0)),
                new(wideItem, new Vector2Int(1, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced entryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 0));
            long hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            createFlow(combatContext, attacker, entryPoint);

            // when
            attacker.command().combatTick(
                CombatTicks.of(ShapeCastEntryCastTicks + ShapeCastWideItemBaseCastTicks * 2 - 1),
                combatContext.getCombatCapabilities());

            // then
            Assert.AreEqual(
                hpBefore,
                TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(1, attacker.query().getActiveFlowCount());

            // when
            attacker.command().combatTick(
                CombatTicks.ONE,
                combatContext.getCombatCapabilities());

            // then
            Assert.AreEqual(
                hpBefore - TestHelpers.getDamageAfterDefaultStability(
                    ShapeCastEntryDamage + ShapeCastWideItemDamage),
                TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(0, attacker.query().getActiveFlowCount());
        }

        [Test]
        public void should_expose_domain_cast_state_for_current_processing_row() {
            // given
            IItemDefinition entry = new ShapeCastEntryPointDefinition();
            IItemDefinition wideItem = new ShapeCastWideBattleItemDefinition();

            EquipItemCommand[] attackerItems = {
                new(entry, new Vector2Int(0, 0)),
                new(wideItem, new Vector2Int(1, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced entryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 0));
            IGridItemPlaced widePlacedItem = getPlacedItemAt(attacker, new Vector2Int(1, 0));

            createFlow(combatContext, attacker, entryPoint);

            // when
            attacker.command().combatTick(
                CombatTicks.of(ShapeCastEntryCastTicks + 1),
                combatContext.getCombatCapabilities());

            // then
            var collector = new TestActiveFlowStateCollector();
            attacker.query().collectActiveFlowStates(collector);

            ActiveFlowState flowState = collector.getSingleState();
            ActiveFlowCastState castState = flowState.getCastState();
            Assert.Greater(flowState.getFlowId().Value, 0);
            Assert.AreEqual(widePlacedItem.getId(), castState.getItemId());
            Assert.AreEqual(2, flowState.getProcessingPath().Count);
            Assert.AreEqual(entryPoint.getId(), flowState.getProcessingPath()[0].getItemId());
            Assert.AreEqual(widePlacedItem.getId(), flowState.getProcessingPath()[1].getItemId());
            Assert.AreEqual(0, flowState.getProcessingPath()[0].getLocalRow());
            Assert.AreEqual(0, flowState.getProcessingPath()[1].getLocalRow());
            Assert.IsTrue(flowState.tryGetPreviousProcessingSlot(out var previousProcessingSlot));
            Assert.AreEqual(entryPoint.getId(), previousProcessingSlot.getItemId());
            Assert.AreEqual(0, castState.getProcessingSlot().getLocalRow());
            Assert.AreEqual(2, castState.getProcessingSlot().getCellCount());
            Assert.AreEqual(
                CombatTicks.of(ShapeCastWideItemBaseCastTicks * 2),
                castState.getRequiredCastTicks());
            Assert.AreEqual(
                CombatTicks.of(ShapeCastWideItemBaseCastTicks * 2 - 1),
                castState.getRemainingCastTicks());
        }

        [Test]
        public void should_prepare_flow_processing_state_for_item_equipped_after_combat_context_creation() {
            // given
            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(new EquipItemCommand[] { });

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IItemDefinition entry = new CarryTicksEntryPointDefinition();
            IItemDefinition nextItem = new CarryTicksBattleItemDefinition();

            ICombatCharacterEquippedItem equippedEntry = attacker.command()
                .equipItemOrThrow(new EquipItemCommand(entry, new Vector2Int(0, 0)));
            attacker.command()
                .equipItemOrThrow(new EquipItemCommand(nextItem, new Vector2Int(1, 0)));

            long hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);
            createFlow(combatContext, attacker, equippedEntry);

            // when
            attacker.command().combatTick(
                CombatTicks.of(CarryTestEntryCastTicks + CarryTestNextItemCastTicks),
                combatContext.getCombatCapabilities());

            // then
            long expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                CarryTestEntryDamage + CarryTestNextItemDamage);
            Assert.AreEqual(expectedHp, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(0, attacker.query().getActiveFlowCount());
        }

        [Test]
        public void should_release_processing_slot_after_flow_finishes_so_next_flow_can_use_item() {
            // given
            ICombatContext combatContext = createCarryTicksCombatContext();
            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced entryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 0));
            IGridItemPlaced nextItem = getPlacedItemAt(attacker, new Vector2Int(1, 0));
            long hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            createFlow(combatContext, attacker, entryPoint);
            attacker.command().combatTick(
                CombatTicks.of(CarryTestEntryCastTicks + CarryTestNextItemCastTicks),
                combatContext.getCombatCapabilities());

            createFlow(combatContext, attacker, entryPoint);
            attacker.command().combatTick(
                CombatTicks.of(CarryTestEntryCastTicks + CarryTestNextItemCastTicks),
                combatContext.getCombatCapabilities());

            // then
            long singleFlowDamage = CarryTestEntryDamage + CarryTestNextItemDamage;
            Assert.AreEqual(
                hpBefore - TestHelpers.getDamageAfterDefaultStability(new[] { singleFlowDamage, singleFlowDamage }),
                TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(0, attacker.query().getActiveFlowCount());
            Assert.AreEqual(0, attacker.query().getActiveFlowCountOnItem(nextItem.getId()));
        }

        [Test]
        public void should_continue_and_finish_active_flow_when_last_enemy_is_already_dead() {
            // given
            IItemDefinition killingEntry = new DeathRegressionEntryPointDefinition(1, 1);
            IItemDefinition delayedEntry = new DeathRegressionEntryPointDefinition(5, 1);
            var deathListener = new CountingDeathEventListener();

            EquipItemCommand[] attackerItems = {
                new(killingEntry, new Vector2Int(0, 0)),
                new(delayedEntry, new Vector2Int(2, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withCharacterDeathListener(deathListener)
                .createContext(
                    new CreateCombatCharacterCommand(
                        "Attacker",
                        1_000_000,
                        Team.TeamA,
                        new GridDimensions(17, 8),
                        attackerItems),
                    new CreateCombatCharacterCommand(
                        "Defender",
                        DeathRegressionDefenderHp,
                        Team.TeamB,
                        new GridDimensions(17, 8),
                        new EquipItemCommand[] { }));

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced killingEntryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 0));
            IGridItemPlaced delayedEntryPoint = getPlacedItemAt(attacker, new Vector2Int(2, 0));

            createFlow(combatContext, attacker, killingEntryPoint);
            createFlow(combatContext, attacker, delayedEntryPoint);

            attacker.command().combatTick(CombatTicks.ONE, combatContext.getCombatCapabilities());

            Assert.AreEqual(0, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(1, deathListener.getDeathEventCount());
            Assert.AreEqual(1, attacker.query().getActiveFlowCount());

            // when
            Assert.DoesNotThrow(() => attacker.command().combatTick(
                CombatTicks.of(4),
                combatContext.getCombatCapabilities()));

            // then
            Assert.AreEqual(0, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(1, deathListener.getDeathEventCount());
            Assert.AreEqual(0, attacker.query().getActiveFlowCount());
        }

        [Test]
        public void should_wait_for_entry_point_cast_time_before_dealing_damage() {
            // given
            IItemDefinition entry = new EntryPointGem();

            EquipItemCommand[] attackerItems = {
                new(entry, new Vector2Int(0, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            BattleSession session = BattleSessionTestFixtures.basic(combatContext);
            long hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickMany(new ManualBattleLoop(), EntryPointTriggerTicks + EntryPointCastTicks - 1);

            // then
            Assert.AreEqual(hpBefore, TestHelpers.getTeamHp(combatContext, Team.TeamB));

            // when
            session.tickOnce();

            // then
            long expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                TestHelpers.getDamage(new[] { entry }));
            Assert.AreEqual(expectedHp, TestHelpers.getTeamHp(combatContext, Team.TeamB));
        }

        [Test]
        public void should_carry_unused_ticks_to_next_item_in_same_flow_tick() {
            // given
            ICombatContext combatContext = createCarryTicksCombatContext();
            ICombatCharacterFacade attacker = createCarryTicksFlow(combatContext);
            long hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            attacker.command().combatTick(
                CombatTicks.of(CarryTestEntryCastTicks + CarryTestNextItemCastTicks),
                combatContext.getCombatCapabilities());

            // then
            long expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                CarryTestEntryDamage + CarryTestNextItemDamage);
            Assert.AreEqual(expectedHp, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(0, attacker.query().getActiveFlowCount());
        }

        [Test]
        public void should_keep_partial_cast_progress_between_ticks() {
            // given
            ICombatContext combatContext = createCarryTicksCombatContext();
            ICombatCharacterFacade attacker = createCarryTicksFlow(combatContext);
            long hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            attacker.command().combatTick(
                CombatTicks.of(CarryTestEntryCastTicks + CarryTestNextItemCastTicks - 1),
                combatContext.getCombatCapabilities());

            // then
            Assert.AreEqual(hpBefore, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(1, attacker.query().getActiveFlowCount());

            // when
            attacker.command().combatTick(
                CombatTicks.ONE,
                combatContext.getCombatCapabilities());

            // then
            long expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                CarryTestEntryDamage + CarryTestNextItemDamage);
            Assert.AreEqual(expectedHp, TestHelpers.getTeamHp(combatContext, Team.TeamB));
            Assert.AreEqual(0, attacker.query().getActiveFlowCount());
        }

        [Test]
        public void should_process_same_result_for_batched_and_split_ticks() {
            // given
            ICombatContext batchedContext = createCarryTicksCombatContext();
            ICombatCharacterFacade batchedAttacker = createCarryTicksFlow(batchedContext);

            ICombatContext splitContext = createCarryTicksCombatContext();
            ICombatCharacterFacade splitAttacker = createCarryTicksFlow(splitContext);

            // when
            batchedAttacker.command().combatTick(
                CombatTicks.of(CarryTestEntryCastTicks + CarryTestNextItemCastTicks),
                batchedContext.getCombatCapabilities());

            for (int i = 0; i < CarryTestEntryCastTicks + CarryTestNextItemCastTicks; i++) {
                splitAttacker.command().combatTick(
                    CombatTicks.ONE,
                    splitContext.getCombatCapabilities());
            }

            // then
            Assert.AreEqual(
                TestHelpers.getTeamHp(batchedContext, Team.TeamB),
                TestHelpers.getTeamHp(splitContext, Team.TeamB));
            Assert.AreEqual(
                batchedAttacker.query().getActiveFlowCount(),
                splitAttacker.query().getActiveFlowCount());
        }

        [Test]
        public void should_skip_item_when_flow_processing_capacity_is_full() {
            // given
            IItemDefinition entry1 = new CapacityTestEntryPointDefinition();
            IItemDefinition entry2 = new CapacityTestEntryPointDefinition();
            IItemDefinition sharedItem = new CapacityTestBattleItemDefinition();

            EquipItemCommand[] attackerItems = {
                new(entry1, new Vector2Int(0, 0)),
                new(sharedItem, new Vector2Int(0, 1)),
                new(entry2, new Vector2Int(0, 2))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);

            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced firstEntryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 0));
            IGridItemPlaced secondEntryPoint = getPlacedItemAt(attacker, new Vector2Int(0, 2));
            IGridItemPlaced sharedPlacedItem = getPlacedItemAt(attacker, new Vector2Int(0, 1));

            combatContext.getCombatCapabilities()
                .command()
                .dispatch(new CreateFlowCombatCommand(
                    attacker.query().getCharacterInfo().getCharacterId(),
                    firstEntryPoint.getId()));

            combatContext.getCombatCapabilities()
                .command()
                .dispatch(new CreateFlowCombatCommand(
                    attacker.query().getCharacterInfo().getCharacterId(),
                    secondEntryPoint.getId()));

            // when
            attacker.command().combatTick(
                CombatTicks.of(CapacityTestEntryCastTicks),
                combatContext.getCombatCapabilities());

            // then
            Assert.AreEqual(1, attacker.query().getActiveFlowCount());
            Assert.AreEqual(1, attacker.query().getActiveFlowCountOnItem(sharedPlacedItem.getId()));
        }

        [Test]
        public void should_move_item_by_hammer_and_not_deal_damage_by_shield() {
            // given
            IItemDefinition entry = new EntryPointGem();
            IItemDefinition sword = new RustySword();
            IItemDefinition hammer = new Hammer();
            IItemDefinition shield = new Shield();

            EquipItemCommand[] itemCommands = {
                new(entry, new Vector2Int(0, 0)),
                new(sword, new Vector2Int(1, 0)),
                new(hammer, new Vector2Int(3, 2)),
                new(shield, new Vector2Int(5, 4))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(itemCommands);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickMany(new ManualBattleLoop(), SingleFlowResolutionTicksBeforeNextTrigger);

            // then
            IItemDefinition[] expectedDamageSources = {
                entry, sword, hammer
            };

            var expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                TestHelpers.getDamage(expectedDamageSources));
            var hpAfter = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            Assert.AreEqual(expectedHp, hpAfter);
        }

        [Test]
        public void should_deal_damage_to_defender() {
            // given
            IItemDefinition entry = new EntryPointGem();
            IItemDefinition sword = new RustySword();
            IItemDefinition shield = new Shield();
            IItemDefinition hammer = new Hammer();

            EquipItemCommand[] attackerItems = {
                new(entry, new Vector2Int(0, 0)),
                new(sword, new Vector2Int(1, 0)),
                new(shield, new Vector2Int(3, 1)),
                new(hammer, new Vector2Int(6, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(attackerItems);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickMany(new ManualBattleLoop(), SingleFlowResolutionTicksBeforeNextTrigger);

            // then
            IItemDefinition[] expectedDamageSources = {
                entry, sword, shield, hammer
            };

            var expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                TestHelpers.getDamage(expectedDamageSources));
            var hpAfter = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            Assert.AreEqual(expectedHp, hpAfter);
        }

        [Test]
        public void should_deal_damage_by_two_entry_points() {
            // given
            IItemDefinition entry1 = new EntryPointGem();
            IItemDefinition entry2 = new EntryPointGem();

            EquipItemCommand[] attackerItems = {
                new(entry1, new Vector2Int(0, 0)),
                new(entry2, new Vector2Int(3, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(attackerItems);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickMany(new ManualBattleLoop(), 31);

            // then
            IItemDefinition[] expectedDamageSources = {
                entry1, entry2
            };

            var expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                expectedDamageSources.Select(item => TestHelpers.getDamage(new[] { item })));
            var hpAfter = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            Assert.AreEqual(expectedHp, hpAfter);
        }

        [Test]
        public void should_deal_damage_when_reduce_damage_to_0_shield_flow() {
            // given
            IItemDefinition entry = new EntryPointGem();
            IItemDefinition shield = new Shield();
            IItemDefinition sword = new RustySword();

            EquipItemCommand[] attackerItems = {
                new(entry, new Vector2Int(0, 0)),
                new(shield, new Vector2Int(1, 0)),
                new(sword, new Vector2Int(3, 0))
            };

            ICombatContext combatContext = BattleScenarioTestHarness.create()
                .withInstantFlowItemCastTime()
                .create1V1WithEnormousHp(attackerItems);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickMany(new ManualBattleLoop(), SingleFlowResolutionTicksBeforeNextTrigger);

            // then
            IItemDefinition[] expectedDamageSources = {
                entry, shield, sword
            };

            var expectedHp = hpBefore - TestHelpers.getDamageAfterDefaultStability(
                TestHelpers.getDamage(expectedDamageSources));
            var hpAfter = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            Assert.AreEqual(expectedHp, hpAfter);
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
            ICombatCharacterFacade attacker,
            IGridItemPlaced entryPointItem) {
            combatContext.getCombatCapabilities()
                .command()
                .dispatch(new CreateFlowCombatCommand(
                    attacker.query().getCharacterInfo().getCharacterId(),
                    entryPointItem.getId()));
        }

        private static ICombatContext createCarryTicksCombatContext() {
            IItemDefinition entry = new CarryTicksEntryPointDefinition();
            IItemDefinition nextItem = new CarryTicksBattleItemDefinition();

            EquipItemCommand[] attackerItems = {
                new(entry, new Vector2Int(0, 0)),
                new(nextItem, new Vector2Int(1, 0))
            };

            return BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(attackerItems);
        }

        private static ICombatCharacterFacade createCarryTicksFlow(ICombatContext combatContext) {
            ICombatCharacterFacade attacker = getCharacterByTeam(combatContext, Team.TeamA);
            IGridItemPlaced entryPointItem = getPlacedItemAt(attacker, new Vector2Int(0, 0));

            createFlow(combatContext, attacker, entryPointItem);

            return attacker;
        }

        private sealed class CarryTicksEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new CarryTicksActionDescription(
                    ItemCastTime.ofTicks(CarryTestEntryCastTicks),
                    CarryTestEntryDamage);
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

        private sealed class CarryTicksBattleItemDefinition : IItemDefinition {
            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new CarryTicksActionDescription(
                    ItemCastTime.ofTicks(CarryTestNextItemCastTicks),
                    CarryTestNextItemDamage);
            }
        }

        private sealed class CarryTicksActionDescription : IActionDescription {
            private readonly ItemCastTime castTime;
            private readonly int damage;

            public CarryTicksActionDescription(ItemCastTime castTime, int damage) {
                this.castTime = castTime;
                this.damage = damage;
            }

            public ItemCastTime getCastTime() {
                return castTime;
            }

            public IOperations getEffectsDescriptor() {
                return new CarryTicksOperations(damage);
            }
        }

        private sealed class CapacityTestEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new CarryTicksActionDescription(
                    ItemCastTime.ofTicks(CapacityTestEntryCastTicks),
                    CapacityTestEntryDamage);
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

        private sealed class CapacityTestBattleItemDefinition : IItemDefinition {
            private readonly ShapeArchetype shape;

            internal CapacityTestBattleItemDefinition() {
                shape = ShapeCatalog.Square1x1;
            }

            internal CapacityTestBattleItemDefinition(ShapeArchetype shape) {
                this.shape = shape;
            }

            public ShapeArchetype getShape() {
                return shape;
            }

            public IActionDescription getActionDescription() {
                return new CarryTicksActionDescription(
                    ItemCastTime.ofTicks(CapacityTestSharedItemCastTicks),
                    CapacityTestSharedItemDamage);
            }
        }

        private sealed class ShapeCastEntryPointDefinition : IEntryPointDefinition {
            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new CarryTicksActionDescription(
                    ItemCastTime.ofTicks(ShapeCastEntryCastTicks),
                    ShapeCastEntryDamage);
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

        private sealed class ShapeCastWideBattleItemDefinition : IItemDefinition {
            public ShapeArchetype getShape() {
                return ShapeCatalog.Square2x2;
            }

            public IActionDescription getActionDescription() {
                return new CarryTicksActionDescription(
                    ItemCastTime.ofTicks(ShapeCastWideItemBaseCastTicks),
                    ShapeCastWideItemDamage);
            }
        }

        private sealed class DeathRegressionEntryPointDefinition : IEntryPointDefinition {
            private readonly int castTicks;
            private readonly int damage;

            internal DeathRegressionEntryPointDefinition(int castTicks, int damage) {
                this.castTicks = castTicks;
                this.damage = damage;
            }

            public ShapeArchetype getShape() {
                return ShapeCatalog.Square1x1;
            }

            public IActionDescription getActionDescription() {
                return new CarryTicksActionDescription(
                    ItemCastTime.ofTicks(castTicks),
                    damage);
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

        private sealed class CarryTicksOperations : IOperations {
            private readonly IOperation[] effects;

            public CarryTicksOperations(int damage) {
                effects = new IOperation[] {
                    new AddPower(DamageRole.ATTACK, new DamageToDeal(damage))
                };
            }

            public IReadOnlyList<IOperation> getEffects() {
                return effects;
            }
        }

        private sealed class TestActiveFlowStateCollector : IActiveFlowStateCollector {
            private readonly List<ActiveFlowState> states = new();

            public void addActiveFlowState(ActiveFlowState flowState) {
                states.Add(flowState);
            }

            internal ActiveFlowState getSingleState() {
                Assert.AreEqual(1, states.Count);
                return states[0];
            }

            internal IReadOnlyList<ActiveFlowState> getStates() {
                return states;
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