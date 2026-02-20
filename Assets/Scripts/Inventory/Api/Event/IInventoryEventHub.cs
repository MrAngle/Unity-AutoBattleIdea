using MageFactory.Inventory.Api.Event.Dto;

namespace MageFactory.Inventory.Api.Event {
    public interface IInventoryEventPublisher {
        void publish(in NewItemPlacedDtoEvent ev);
    }

    public interface IInventoryEventRegistry {
        void subscribe(IItemPlacedEventListener listener);
        void unsubscribe(IItemPlacedEventListener listener);
    }
}