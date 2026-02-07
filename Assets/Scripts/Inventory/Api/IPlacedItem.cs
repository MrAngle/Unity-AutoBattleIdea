using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Controller.Api {
    public interface IPlacedItem : IInventoryPosition {
        public long getId();
        public IActionDescription prepareItemActionDescription();
        public ShapeArchetype getShape();
    }
}