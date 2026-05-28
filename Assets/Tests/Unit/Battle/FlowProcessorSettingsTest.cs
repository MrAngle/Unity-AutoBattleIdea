using System;
using MageFactory.Flow.Configuration;
using NUnit.Framework;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class FlowProcessorSettingsTest {
        [Test]
        public void should_reject_non_positive_max_steps_per_slice() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FlowProcessorSettings(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FlowProcessorSettings(-1));
        }

        [Test]
        public void should_keep_configured_max_steps_per_slice() {
            var settings = new FlowProcessorSettings(3);

            Assert.AreEqual(3, settings.getMaxStepsPerSlice());
        }
    }
}