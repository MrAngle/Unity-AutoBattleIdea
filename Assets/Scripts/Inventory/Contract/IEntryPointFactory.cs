using MageFactory.CombatContext.Contract;

namespace MageFactory.Inventory.Contract {
    public interface IEntryPointFactory {
        // IEntryPointArchetype createArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype);

        // IInventoryPlacedEntryPoint createPlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
        //     IInventoryInspector inventoryInspector);

        public IInventoryPlacedEntryPoint createPlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition,
            ICharacterCombatCapabilities characterCombatCapabilities
        );
    }
}