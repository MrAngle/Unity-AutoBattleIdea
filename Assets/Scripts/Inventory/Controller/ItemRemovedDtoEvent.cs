namespace MageFactory.Inventory.Controller {
    internal readonly struct ItemRemovedDtoEvent {
        internal long PlacedItemId { get; }

        internal ItemRemovedDtoEvent(long id) {
            PlacedItemId = id;
        }
    }
}