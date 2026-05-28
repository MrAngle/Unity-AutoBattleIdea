using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public class EntryPointGem : IEntryPointDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square1x1;
        }

        public IActionDescription getActionDescription() {
            return new EntryPointGemActionDescription();
        }

        public FlowKind getFlowKind() {
            return FlowKind.Damage;
        }

        public CombatTicks getTriggerIntervalTicks() {
            return CombatTicks.of(20);
        }

        private class EntryPointGemActionDescription : IActionDescription {
            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(5);
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription(
                    new AddPower(DamageRole.ATTACK, new PowerAmount(2))
                );
            }
        }
    }
}