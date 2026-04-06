using System.Runtime.CompilerServices;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Shared.Event;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal sealed class InventoryEventHub : IInventoryEventPublisher, IInventoryEventRegistry {
        private readonly DomainEventChannel<NewItemPlacedDtoEvent, IItemPlacedEventEventListener> itemPlacedChannel
            = new();

        private readonly DomainEventChannel<ItemPositionChangedDtoEvent, IItemPositionChangedEventListener>
            itemPositionChangedChannel
                = new();

        public void subscribe(IItemPlacedEventEventListener eventListener) =>
            itemPlacedChannel.subscribe(eventListener);

        public void unsubscribe(IItemPlacedEventEventListener eventListener) =>
            itemPlacedChannel.unsubscribe(eventListener);

        public void subscribe(IItemPositionChangedEventListener eventListener) {
            itemPositionChangedChannel.subscribe(eventListener);
        }

        public void unsubscribe(IItemPositionChangedEventListener eventListener) {
            itemPositionChangedChannel.unsubscribe(eventListener);
        }

        public void publish(in NewItemPlacedDtoEvent ev) =>
            itemPlacedChannel.publish(in ev);

        public void publish(in ItemPositionChangedDtoEvent ev) {
            itemPositionChangedChannel.publish(in ev);
        }
    }
}