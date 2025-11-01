// using UnityEngine;
//
// namespace Combat.Flow.Domain.Aggregate {
//     public class DummyFlowNode : IFlowNode {
//         private long _nodeId;
//         
//         internal DummyFlowNode(Vector2Int vector2Int, long id) {
//             _nodeId = id;
//         }
//
//         public long GetId() {
//             return _nodeId;
//         }
//
//         public void Process(FlowAggregate flowAggregate) {
//             // Przykład: +5 power na każdym węźle (na razie cokolwiek)
//             flowAggregate.GetModel().FlowPayload.Add(5);
//         }
//     }
// }