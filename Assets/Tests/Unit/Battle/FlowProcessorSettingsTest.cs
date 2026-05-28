using System;
using MageFactory.Flow.Configuration;
using NUnit.Framework;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class FlowProcessorSettingsTest {
        [Test]
        public void should_reject_non_positive_max_steps_per_slice() {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new FlowProcessorSettings(0, FlowCastTimeMode.UseItemCastTime));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new FlowProcessorSettings(-1, FlowCastTimeMode.UseItemCastTime));
        }

        [Test]
        public void should_reject_undefined_cast_time_mode() {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new FlowProcessorSettings(1, (FlowCastTimeMode)999));
        }

        [Test]
        public void should_keep_configured_max_steps_per_slice() {
            FlowProcessorSettings settings = new FlowProcessorSettings(3, FlowCastTimeMode.Instant);

            Assert.AreEqual(3, settings.getMaxStepsPerSlice());
            Assert.AreEqual(FlowCastTimeMode.Instant, settings.getCastTimeMode());
        }
    }
}