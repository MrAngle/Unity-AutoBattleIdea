namespace MageFactory.Character.Contract.Event {
    public readonly struct ItemPowerChangedDtoEvent {
        public long ItemId { get; }
        public long Delta { get; }

        public ItemPowerChangedDtoEvent(long id, long delta) {
            ItemId = id;
            Delta = delta;
        }
    }
}