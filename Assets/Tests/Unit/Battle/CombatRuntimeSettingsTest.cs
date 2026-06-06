using System;
using MageFactory.CombatContextRuntime;
using NUnit.Framework;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class CombatRuntimeSettingsTest {
        [Test]
        public void should_enable_hot_path_logs_for_developer_profile() {
            CombatRuntimeSettings settings = CombatRuntimeSettings.developer();

            Assert.AreEqual(CombatRuntimeProfile.Developer, settings.getProfile());
            Assert.IsTrue(settings.shouldLogCombatHotPath());
        }

        [Test]
        public void should_disable_hot_path_logs_for_production_profile() {
            CombatRuntimeSettings settings = CombatRuntimeSettings.production();

            Assert.AreEqual(CombatRuntimeProfile.Production, settings.getProfile());
            Assert.IsFalse(settings.shouldLogCombatHotPath());
        }

        [Test]
        public void should_reject_unsupported_combat_runtime_profile() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CombatRuntimeSettings((CombatRuntimeProfile)999));
        }
    }
}