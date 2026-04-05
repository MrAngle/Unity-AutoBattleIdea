using MageFactory.Shared.Contract;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public class RustySword : IItemDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.StandardSword;
        }
    }
}