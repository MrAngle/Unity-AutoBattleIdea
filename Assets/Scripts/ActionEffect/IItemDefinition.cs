using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.ActionEffect {
    public interface IItemDefinition {
        ShapeArchetype getShape();
        public IActionDescription getActionDescription();
    }

    public interface IEntryPointDefinition : IItemDefinition {
        FlowKind getFlowKind();

        CombatTicks getTriggerIntervalTicks();
    }
}