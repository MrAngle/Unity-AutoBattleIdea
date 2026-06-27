using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public sealed class StabilityCore : IItemDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square1x1;
        }

        public IActionDescription getActionDescription() {
            return new StabilityCoreActionDescription();
        }

        private sealed class StabilityCoreActionDescription : IActionDescription {
            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(3);
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription(
                    new AddStabilityPower(new StabilityPower(2)));
            }
        }
    }
}