namespace MageFactory.Character.Contract {
    public interface IInventoryFactory {
        ICharacterInventory createCharacterInventory();

        // public IPlacedEntryPoint createPlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
        //     IGridInspector gridInspector);

        // @Override
        // public IEntryPointArchetype createArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype);
    }
}