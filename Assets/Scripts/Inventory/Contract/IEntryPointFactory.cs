namespace MageFactory.Inventory.Contract {
    public interface IEntryPointFactory {
        public IInventoryPlacedEntryPoint createPlacedEntryPoint(
            IEntryPointArchetype entryPointArchetype,
            IInventoryPosition inventoryPosition
        );
    }
}