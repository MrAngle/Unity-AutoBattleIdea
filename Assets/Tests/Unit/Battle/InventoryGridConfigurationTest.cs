using MageFactory.Inventory.Contract;
using MageFactory.Shared.Model;
using NUnit.Framework;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class InventoryGridConfigurationTest {
        [Test]
        public void should_allow_grid_dimensions_within_configured_maximum() {
            InventoryGridConfiguration configuration = new InventoryGridConfiguration(
                new GridDimensions(100, 50));

            Assert.IsTrue(configuration.canUse(new GridDimensions(17, 8)));
            Assert.IsTrue(configuration.canUse(new GridDimensions(100, 50)));
        }

        [Test]
        public void should_reject_grid_dimensions_exceeding_configured_maximum() {
            InventoryGridConfiguration configuration = new InventoryGridConfiguration(
                new GridDimensions(100, 50));

            Assert.IsFalse(configuration.canUse(new GridDimensions(101, 50)));
            Assert.IsFalse(configuration.canUse(new GridDimensions(100, 51)));
        }
    }
}