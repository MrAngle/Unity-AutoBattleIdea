using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Api.Dto {
    public record CreatePlaceableItemCommand(ShapeArchetype shapeArchetype);
}