using MageFactory.ActionEffect;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Item.Catalog.Bases;
using MageFactory.Shared.Model;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class DeterministicFlowScenarioTest {
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
                .withInstantActionExecutorInstance()
                .create1V1WithEnormousHp(itemCommands);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickOnce();

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
                .withInstantActionExecutorInstance()
                .create1V1WithEnormousHp(attackerItems);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickOnce();

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
                .withInstantActionExecutorInstance()
                .create1V1WithEnormousHp(attackerItems);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickOnce();

            // then
            IItemDefinition[] expectedDamageSources = {
                entry1, entry2
            };

            var expectedHp = hpBefore - (TestHelpers.getDamage(expectedDamageSources) * 2);
            var hpAfter = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            Assert.AreEqual(expectedHp, hpAfter);
        }

        [Test]
        public void should_deal_2_damage_when_shield_is_second_in_flow() {
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
                .withInstantActionExecutorInstance()
                .create1V1WithEnormousHp(attackerItems);

            var session = BattleSessionTestFixtures.basic(combatContext);
            var hpBefore = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            // when
            session.tickOnce();

            // then
            IItemDefinition[] expectedDamageSources = {
                entry, shield, sword
            };

            var expectedHp = hpBefore - TestHelpers.getDamage(expectedDamageSources);
            var hpAfter = TestHelpers.getTeamHp(combatContext, Team.TeamB);

            Assert.AreEqual(expectedHp, hpAfter);
        }
    }
}