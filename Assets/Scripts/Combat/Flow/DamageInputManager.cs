using Combat.Flow.Domain.Aggregate;
using UnityEngine;
using Zenject;

namespace Combat.Flow.Domain
{
    public interface IFlowStarter
    {
        /// Uruchamia przepływ od podanego węzła startowego i zwraca agregat.
        FlowAggregate StartFlow(FlowKind kind, long power, Vector2Int startNode, string sourceId);
    }

    public class DamageInputManager : IFlowStarter
    {
        public FlowAggregate StartFlow(FlowKind kind, long power, Vector2Int startNode, string sourceId) {
            return FlowAggregate.Start(kind, power, startNode, sourceId);
        }
    }
}