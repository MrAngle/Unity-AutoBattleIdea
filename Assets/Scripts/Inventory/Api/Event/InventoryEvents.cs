using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Shared.Event;

namespace MageFactory.Inventory.Api.Event {
    public interface IItemPlacedEventEventListener
        : IDomainEventListener<NewItemPlacedDtoEvent> {
    }
}