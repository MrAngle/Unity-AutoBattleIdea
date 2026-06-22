using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.CombatEvents;
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

        public EntryPointTriggerKind getTriggerKind() {
            return EntryPointTriggerKind.CombatTick;
        }

        public ICombatHook getCombatHook() {
            return CombatHook.none();
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

    public class DefenseEntryPointGem : IEntryPointDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square1x1;
        }

        public IActionDescription getActionDescription() {
            return new DefenseEntryPointGemActionDescription();
        }

        public FlowKind getFlowKind() {
            return FlowKind.Defense;
        }

        public EntryPointTriggerKind getTriggerKind() {
            return EntryPointTriggerKind.CombatEvent;
        }

        public ICombatHook getCombatHook() {
            return CombatHook.onIncomingAttackDamage();
        }

        public CombatTicks getTriggerIntervalTicks() {
            return CombatTicks.ONE;
        }

        private class DefenseEntryPointGemActionDescription : IActionDescription {
            public ItemCastTime getCastTime() {
                return ItemCastTime.ofTicks(5);
            }

            public IOperations getEffectsDescriptor() {
                return new ItemOperationsDescription(
                    new AddGuardPower(new GuardPower(2))
                );
            }
        }
    }
}