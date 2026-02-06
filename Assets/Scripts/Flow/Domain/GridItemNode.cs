namespace MageFactory.Flow.Domain {
    // public sealed class GridStartNode : IFlowNode {
    //     public GridStartNode(string startId, Vector2Int position) {
    //         NodeId = $"Start:{startId}@{position.x},{position.y}";
    //         Position = position;
    //     }
    //
    //     public string NodeId { get; }
    //     public Vector2Int Position { get; }
    //
    //     public long GetId() {
    //         throw new System.NotImplementedException();
    //     }
    //
    //     public void Process(FlowAggregate model) {
    //         /* start nic nie zmienia */
    //     }
    //
    //     public ICollection<Vector2Int> GetPosition() {
    //         throw new System.NotImplementedException();
    //     }
    // }

    // public sealed class GridItemNode : IFlowNode {
    //     public GridItemNode(ItemData item, Vector2Int origin) {
    //         Item = item;
    //         Origin = origin;
    //         NodeId = $"Item:{item.ItemDataId}";
    //     }
    //
    //     public string NodeId { get; }
    //     public ItemData Item { get; }
    //     public Vector2Int Origin { get; } // potrzebny do obliczeń shape
    //
    //     public void Process(FlowModel model) {
    //         // TODO: efekt przedmiotu (na razie puste)
    //     }
    //
    //     public long GetId() {
    //         throw new System.NotImplementedException();
    //     }
    //
    //     public void Process(FlowAggregate flowAggregate) {
    //         throw new System.NotImplementedException();
    //     }
    // }
}