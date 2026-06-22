using MageFactory.Shared.Model;

namespace MageFactory.Shared.Contract {
    public interface IFlowPortPlacedItem {
        FlowPortKind getFlowPortKind();
        string getFlowPortName();
        string getFlowPortDescription();
    }
}