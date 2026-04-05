using MageFactory.ActionEffect;

namespace MageFactory.Inventory.Contract.Dto {
    public record CreatePlaceableItemCommand(IItemDefinition itemDefinition);
}