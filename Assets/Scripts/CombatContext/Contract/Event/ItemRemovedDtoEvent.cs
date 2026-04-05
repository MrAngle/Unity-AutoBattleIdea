using MageFactory.Shared.Id;

namespace MageFactory.Character.Contract.Event {
    public readonly struct ItemRemovedDtoEvent {
        public Id<ItemId> PlacedItemId { get; }

        public ItemRemovedDtoEvent(Id<ItemId> id) {
            PlacedItemId = id;
        }
    }
}