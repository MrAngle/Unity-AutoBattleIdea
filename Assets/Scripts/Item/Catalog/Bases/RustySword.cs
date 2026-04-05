using System;
using MageFactory.ActionEffect;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public class RustySword : IItemDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.StandardSword;
        }

        public IActionDescription getActionDescription() {
            throw new NotImplementedException();
        }
    }
}