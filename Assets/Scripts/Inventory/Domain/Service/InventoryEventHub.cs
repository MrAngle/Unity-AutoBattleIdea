using System.Runtime.CompilerServices;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Shared.Event;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal sealed class InventoryEventHub : IInventoryEventPublisher, IInventoryEventRegistry {
        private readonly DomainEventChannel<NewItemPlacedDtoEvent, IItemPlacedEventEventListener> itemPlacedChannel
            = new();

        public void subscribe(IItemPlacedEventEventListener eventListener) =>
            itemPlacedChannel.subscribe(eventListener);

        public void unsubscribe(IItemPlacedEventEventListener eventListener) =>
            itemPlacedChannel.unsubscribe(eventListener);

        public void publish(in NewItemPlacedDtoEvent ev) =>
            itemPlacedChannel.publish(in ev);
    }
}