using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public class Shield : IItemDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square2x2;
        }

        public IActionDescription getActionDescription() {
            return new ShieldActionDescription();
        }

        private class ShieldActionDescription : IActionDescription {
            public Duration getCastTime() {
                return new Duration(0.25f);
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription(
                    new AddPower(DamageRole.ATTACK, new PowerAmount(-5))
                );
            }
        }
    }
}