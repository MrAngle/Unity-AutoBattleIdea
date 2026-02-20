using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Shared.Event.Listener;

namespace MageFactory.Inventory.Api.Event {
    public interface IItemPlacedEventListener
        : IDomainListener<NewItemPlacedDtoEvent> {
    }
}