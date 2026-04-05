using MageFactory.Shared.Contract;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public class Shield : IItemDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square2x2;
        }
    }
}