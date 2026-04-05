using MageFactory.Shared.Id;

namespace MageFactory.Character.Contract.Event {
    public readonly struct ItemPowerChangedDtoEvent {
        public Id<ItemId> ItemId { get; }
        public long Delta { get; }

        public ItemPowerChangedDtoEvent(Id<ItemId> id, long delta) {
            ItemId = id;
            Delta = delta;
        }
    }
}