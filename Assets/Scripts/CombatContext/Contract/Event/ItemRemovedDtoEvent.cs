namespace MageFactory.Character.Contract.Event {
    public readonly struct ItemRemovedDtoEvent {
        public long PlacedItemId { get; }

        public ItemRemovedDtoEvent(long id) {
            PlacedItemId = id;
        }
    }
}