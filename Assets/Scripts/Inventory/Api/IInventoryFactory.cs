namespace MageFactory.Item.Controller.Api {
    public interface IInventoryFactory {
        ICharacterInventoryFacade CreateCharacterInventory();

        // public IPlacedEntryPoint createPlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
        //     IGridInspector gridInspector);

        // @Override
        // public IEntryPointArchetype createArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype);
    }
}