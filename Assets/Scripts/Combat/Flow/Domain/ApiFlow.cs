using System.Collections.Generic;
using Combat.Flow.Domain.Aggregate;

namespace Combat.Flow.Domain
{
    /// Reprezentuje pojedynczy “krok”/węzeł w sieci (np. przedmiot).
    internal interface IFlowNode {
        public long GetId();

        /// Mutuje model (np. wzmacnia power, konwertuje, itp.).
        public void Process(FlowModel model);
    }

    /// Dostarcza następne węzły dla danego kroku (logika trasy).
    internal interface IFlowRouter
    {
        /// Zwraca listę kolejnych węzłów, do których ma popłynąć model.
        internal IEnumerable<IFlowNode> GetNextNodes(IFlowNode current, FlowModel model);
        
        /// Warunek zakończenia (np. brak wyjść, limit kroków, payload ~= 0).
        internal bool ShouldStop(IFlowNode current, FlowModel model);
    }
}