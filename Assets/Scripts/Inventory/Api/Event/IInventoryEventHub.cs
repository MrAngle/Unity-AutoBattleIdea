namespace MageFactory.Inventory.Api.Event {
    public interface IInventoryEventHub {
        public void subscribe(IInventoryChangedEventListener inventoryEventListener);
        public void subscribe(IInventoryItemPlacedEventListener inventoryEventListener);

        public void enqueue(in InventoryChanged ev);
        public void enqueue(in NewItemPlacedDtoEvent ev);

        public void publishAll();
    }
}