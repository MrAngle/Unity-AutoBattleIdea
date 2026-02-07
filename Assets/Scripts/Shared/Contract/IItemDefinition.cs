using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Shared.Contract {
    public interface IItemDefinition {
        ShapeArchetype getShape();
    }

    public interface IEntryPointDefinition : IItemDefinition {
        ShapeArchetype getShape();
        FlowKind getFlowKind();
    }
}