using MageFactory.Item.Api.Dto;
using MageFactory.Item.Controller.Api;

namespace MageFactory.Item.Api {
    public interface IItemFactory {
        IPlaceableItem createPlacableItem(CreatePlaceableItemCommand createPlaceableItemCommand);
    }
}