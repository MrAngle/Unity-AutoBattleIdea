using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.Shared.Contract;
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
            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(20);
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription(
                    new AddPower(DamageRole.ATTACK, new DamageToDeal(5))
                );
            }
        }
    }
}