namespace MageFactory.Inventory.Api {
    public interface IInventoryAggregateFactory {
        ICharacterInventoryFacade CreateCharacterInventory();
    }
}