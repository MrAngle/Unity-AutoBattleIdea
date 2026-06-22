using MageFactory.UI.Component.Inventory.ItemLayer;
using NUnit.Framework;

namespace MageFactory.Tests.Unit {
    public sealed class GuardPowerLabelFormatterTest {
        [TestCase(0, "0")]
        [TestCase(999, "999")]
        [TestCase(1_000, "1k")]
        [TestCase(2_500, "2.5k")]
        [TestCase(12_500, "13k")]
        [TestCase(1_000_000, "1m")]
        [TestCase(2_500_000, "2.5m")]
        public void should_format_guard_power_as_short_inventory_label(long value, string expectedLabel) {
            Assert.AreEqual(expectedLabel, GuardPowerLabelFormatter.format(value));
        }
    }
}