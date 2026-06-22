using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.CombatEvents;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public sealed class PulseInputPort : IEntryPointDefinition, IFlowPortDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square1x1;
        }

        public IActionDescription getActionDescription() {
            return new PulseInputPortActionDescription();
        }

        public FlowKind getFlowKind() {
            return FlowKind.Damage;
        }

        public EntryPointTriggerKind getTriggerKind() {
            return EntryPointTriggerKind.CombatTick;
        }

        public ICombatHook getCombatHook() {
            return CombatHook.none();
        }

        public CombatTicks getTriggerIntervalTicks() {
            return CombatTicks.of(20);
        }

        public FlowPortKind getFlowPortKind() {
            return FlowPortKind.Input;
        }

        public string getFlowPortName() {
            return "IN";
        }

        public string getFlowPortDescription() {
            return
                "Starts a port-aware flow every 20 combat ticks. This prototype seeds the flow with a small power signal.";
        }

        private sealed class PulseInputPortActionDescription : IActionDescription {
            public ItemCastTime getCastTime() {
                return ItemCastTime.ZERO;
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription(
                    new AddPower(DamageRole.ATTACK, new PowerAmount(2)));
            }
        }
    }

    public sealed class BasicOutputPort : IItemDefinition, IFlowPortDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square1x1;
        }

        public IActionDescription getActionDescription() {
            return new BasicOutputPortActionDescription();
        }

        public FlowPortKind getFlowPortKind() {
            return FlowPortKind.Output;
        }

        public string getFlowPortName() {
            return "OUT";
        }

        public string getFlowPortDescription() {
            return
                "Commits the accumulated flow power. Attack power is fired at a target; guard power creates prepared guards.";
        }

        private sealed class BasicOutputPortActionDescription : IActionDescription {
            public ItemCastTime getCastTime() {
                return ItemCastTime.ZERO;
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription();
            }
        }
    }
}