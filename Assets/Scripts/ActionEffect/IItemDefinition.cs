using MageFactory.CombatEvents;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.ActionEffect {
    public enum EntryPointTriggerKind {
        CombatTick,
        CombatEvent
    }

    public interface IItemDefinition {
        ShapeArchetype getShape();
        public IActionDescription getActionDescription();
    }

    public interface IFlowPortDefinition : IItemDefinition {
        FlowPortKind getFlowPortKind();
        string getFlowPortName();
        string getFlowPortDescription();
    }

    public interface IEntryPointDefinition : IItemDefinition {
        FlowKind getFlowKind();

        EntryPointTriggerKind getTriggerKind();

        ICombatHook getCombatHook();

        CombatTicks getTriggerIntervalTicks();
    }
}