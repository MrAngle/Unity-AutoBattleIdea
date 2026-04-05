using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public class RustySword : IItemDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.StandardSword;
        }

        public IActionDescription getActionDescription() {
            return new RustySwordActionDescription();
        }

        private class RustySwordActionDescription : IActionDescription {
            public Duration getCastTime() {
                return new Duration(0.15f);
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription(
                    new AddPower(new DamageToDeal(5))
                );
            }
        }
    }
}