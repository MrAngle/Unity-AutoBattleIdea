using UnityEngine;

namespace Combat.Flow.Domain.Aggregate {
    internal class DummyFlowNode : IFlowNode {
        private long _nodeId;
        
        internal DummyFlowNode(Vector2Int vector2Int, long id) {
            _nodeId = id;
        }

        public long GetId() {
            return _nodeId;
        }

        public void Process(FlowModel model) {
            // Przykład: +5 power na każdym węźle (na razie cokolwiek)
            model.FlowPayload.Add(5);
        }
    }
}