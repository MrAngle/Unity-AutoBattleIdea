using System.Collections.Generic;
using System.Linq;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.BattleManager;
using MageFactory.CombatContext.Api;
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
    public sealed class DeterministicFlowScenarioTest {
        private const int EntryPointTriggerTicks = 20;
        private const int EntryPointCastTicks = 5;
        private const int SingleFlowResolutionTicksBeforeNextTrigger = 35;
        private const int CarryTestEntryCastTicks = 3;
        private const int CarryTestNextItemCastTicks = 7;
        private const int CarryTestEntryDamage = 2;
        private const int CarryTestNextItemDamage = 5;

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
            long expectedHp = hpBefore - TestHelpers.getDamage(new[] { entry });
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
            long expectedHp = hpBefore - CarryTestEntryDamage - CarryTestNextItemDamage;
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
            long expectedHp = hpBefore - CarryTestEntryDamage - CarryTestNextItemDamage;
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

            var expectedHp = hpBefore - TestHelpers.getDamage(expectedDamageSources);
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

            var expectedHp = hpBefore - TestHelpers.getDamage(expectedDamageSources);
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
                new(entry2, new Vector2Int(1, 0))
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

            var expectedHp = hpBefore - (TestHelpers.getDamage(expectedDamageSources) * 2);
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

            var expectedHp = hpBefore - TestHelpers.getDamage(expectedDamageSources);
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

            combatContext.getCombatCapabilities()
                .command()
                .dispatch(new CreateFlowCombatCommand(
                    attacker.query().getCharacterInfo().getCharacterId(),
                    entryPointItem.getId()));

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
    }
}