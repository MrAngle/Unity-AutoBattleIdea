using MageFactory.Shared.Contract;

namespace MageFactory.Inventory.Contract.Dto {
    public record CreatePlaceableItemCommand(IItemDefinition itemDefinition);
}