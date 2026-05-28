using System;
using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Contract {
    public sealed class InventoryGridConfiguration {
        private readonly GridDimensions maxGridDimensions;

        public InventoryGridConfiguration(GridDimensions maxGridDimensions) {
            this.maxGridDimensions = maxGridDimensions
                                     ?? throw new ArgumentNullException(nameof(maxGridDimensions));
        }

        public GridDimensions getMaxGridDimensions() {
            return maxGridDimensions;
        }

        public bool canUse(GridDimensions gridDimensions) {
            return maxGridDimensions.canContain(gridDimensions);
        }
    }
}