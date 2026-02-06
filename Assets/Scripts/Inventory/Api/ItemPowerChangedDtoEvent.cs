namespace MageFactory.Inventory.Api {
    public readonly struct ItemPowerChangedDtoEvent {
        public long ItemId { get; }
        public long Delta { get; }

        public ItemPowerChangedDtoEvent(long id, long delta) {
            ItemId = id;
            Delta = delta;
        }
    }
}