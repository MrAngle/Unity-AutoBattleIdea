namespace MageFactory.Flow.Domain {
    /// Reprezentuje pojedynczy “krok”/węzeł w sieci (np. przedmiot).
    // public interface IFlowNode {
    //     // public long GetId();
    //     //
    //     // /// Mutuje model (np. wzmacnia power, konwertuje, itp.).
    //     // public void Process(FlowAggregate flowAggregate);
    //     // public IReadOnlyCollection<Vector2Int> GetOccupiedCells();
    // }

    /// Dostarcza następne węzły dla danego kroku (logika trasy).
    // public interface IFlowRouter
    // {
    //     /// Zwraca listę kolejnych węzłów, do których ma popłynąć model.
    //     internal IEnumerable<IFlowNode> GetNextNodes(IFlowNode current, FlowModel model);
    //     
    //     /// Warunek zakończenia (np. brak wyjść, limit kroków, payload ~= 0).
    //     internal bool ShouldStop(IFlowNode current, FlowModel model);
    // }
}