using System;
using MageFactory.BattleManager;
using NUnit.Framework;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class BattleSessionSettingsTest {
        [Test]
        public void should_use_10_combat_ticks_per_real_second_by_default() {
            BattleSessionSettings settings = BattleSessionSettings.createDefault();

            Assert.AreEqual(10, BattleSessionSettings.getDefaultCombatTicksPerRealSecond());
            Assert.AreEqual(10, settings.getCombatTicksPerRealSecond());
        }

        [Test]
        public void should_calculate_real_seconds_per_combat_tick() {
            BattleSessionSettings settings = new BattleSessionSettings(10);

            Assert.AreEqual(0.1f, settings.getRealSecondsPerCombatTick(), 0.0001f);
        }

        [Test]
        public void should_reject_non_positive_combat_ticks_per_real_second() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BattleSessionSettings(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BattleSessionSettings(-1));
        }
    }
}