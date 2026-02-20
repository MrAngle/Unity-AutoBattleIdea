using System.Runtime.CompilerServices;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Shared.Event;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Inventory.Domain.Service {
    internal sealed class InventoryEventHub : IInventoryEventPublisher, IInventoryEventRegistry {
        private readonly InventoryEventChannel<NewItemPlacedDtoEvent, IItemPlacedEventListener> _itemPlacedChannel
            = new();

        public void subscribe(IItemPlacedEventListener listener) =>
            _itemPlacedChannel.subscribe(listener);

        public void unsubscribe(IItemPlacedEventListener listener) =>
            _itemPlacedChannel.unsubscribe(listener);

        public void publish(in NewItemPlacedDtoEvent ev) =>
            _itemPlacedChannel.publish(in ev);
    }
}