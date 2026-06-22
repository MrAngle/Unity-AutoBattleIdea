using System;
using MageFactory.BattleManager;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Item.Catalog.Bases;
using MageFactory.Shared.Model;
using MageFactory.Tests.Unit.TestFixtures;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class BattleTimeControllerTest {
        private const int ComparedCombatTicks = 42;

        [Test]
        public void should_convert_scaled_real_time_to_combat_ticks() {
            BattleTimeController controller = new BattleTimeController(new BattleSessionSettings(10));

            Assert.AreEqual(0, controller.consumeReadyCombatTicks(0.049f));
            Assert.AreEqual(1, controller.consumeReadyCombatTicks(0.051f));

            controller.setSpeedMultiplier(BattleTimeController.MaxSpeedMultiplier);
            Assert.AreEqual(1, controller.consumeReadyCombatTicks(0.02f));

            controller.setSpeedMultiplier(BattleTimeController.MinSpeedMultiplier);
            Assert.AreEqual(0, controller.consumeReadyCombatTicks(0.49f));
            Assert.AreEqual(1, controller.consumeReadyCombatTicks(0.01f));
        }

        [Test]
        public void should_not_accumulate_combat_ticks_while_paused() {
            BattleTimeController controller = new BattleTimeController(new BattleSessionSettings(10));

            controller.pause();
            Assert.AreEqual(0, controller.consumeReadyCombatTicks(10f));

            controller.resume();
            Assert.AreEqual(1, controller.consumeReadyCombatTicks(0.1f));
        }

        [Test]
        public void should_reject_unsupported_speed_values() {
            BattleTimeController controller = new BattleTimeController(new BattleSessionSettings(10));

            Assert.Throws<ArgumentOutOfRangeException>(() => controller.setSpeedMultiplier(0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => controller.setSpeedMultiplier(0.19f));
            Assert.Throws<ArgumentOutOfRangeException>(() => controller.setSpeedMultiplier(5.01f));
            Assert.Throws<ArgumentOutOfRangeException>(() => controller.setSpeedMultiplier(float.NaN));
            Assert.Throws<ArgumentOutOfRangeException>(() => controller.consumeReadyCombatTicks(-0.01f));
        }

        [Test]
        public void changing_battle_speed_should_not_change_combat_logic_for_same_tick_count() {
            ICombatContext manualContext = createPortCombatContext();
            BattleSession manualSession = BattleSessionTestFixtures.basic(manualContext);

            ICombatContext timeDrivenContext = createPortCombatContext();
            BattleSession timeDrivenSession = BattleSessionTestFixtures.basic(timeDrivenContext);
            BattleTimeController controller = new BattleTimeController(BattleSessionSettings.createDefault());

            manualSession.tickMany(new ManualBattleLoop(), ComparedCombatTicks);

            int generatedTicks = driveSessionThroughSpeedChanges(timeDrivenSession, controller);

            Assert.AreEqual(ComparedCombatTicks, generatedTicks);
            Assert.AreEqual(
                TestHelpers.getTeamHp(manualContext, Team.TeamB),
                TestHelpers.getTeamHp(timeDrivenContext, Team.TeamB));
            Assert.AreEqual(
                manualContext.getCombatCapabilities().query().getActiveFlowCount(),
                timeDrivenContext.getCombatCapabilities().query().getActiveFlowCount());
            Assert.AreEqual(
                manualContext.getCombatCapabilities().query().getCreatedFlowCount(),
                timeDrivenContext.getCombatCapabilities().query().getCreatedFlowCount());
        }

        private static int driveSessionThroughSpeedChanges(
            BattleSession session,
            BattleTimeController controller) {
            int generatedTicks = 0;

            controller.setSpeedMultiplier(BattleTimeController.MaxSpeedMultiplier);
            generatedTicks += advanceFrames(session, controller, 0.02f, 10);

            controller.pause();
            generatedTicks += advanceFrames(session, controller, 1f, 1);
            controller.resume();

            controller.setSpeedMultiplier(BattleTimeController.MinSpeedMultiplier);
            generatedTicks += advanceFrames(session, controller, 0.5f, 10);

            controller.setSpeedMultiplier(BattleTimeController.DefaultSpeedMultiplier);
            generatedTicks += advanceFrames(session, controller, 0.1f, 22);

            return generatedTicks;
        }

        private static int advanceFrames(
            BattleSession session,
            BattleTimeController controller,
            float deltaSeconds,
            int frames) {
            int generatedTicks = 0;

            for (int i = 0; i < frames; i++) {
                int readyTicks = controller.consumeReadyCombatTicks(deltaSeconds);
                generatedTicks += readyTicks;

                for (int tick = 0; tick < readyTicks; tick++) {
                    session.tickOnce();
                }
            }

            return generatedTicks;
        }

        private static ICombatContext createPortCombatContext() {
            return BattleScenarioTestHarness.create()
                .create1V1WithEnormousHp(new[] {
                    new EquipItemCommand(new PulseInputPort(), new Vector2Int(0, 0)),
                    new EquipItemCommand(new BasicOutputPort(), new Vector2Int(1, 0))
                });
        }
    }
}