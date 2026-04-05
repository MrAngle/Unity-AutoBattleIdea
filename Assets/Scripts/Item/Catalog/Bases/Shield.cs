using System;
using MageFactory.ActionEffect;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog.Bases {
    public class Shield : IItemDefinition {
        public ShapeArchetype getShape() {
            return ShapeCatalog.Square2x2;
        }

        public IActionDescription getActionDescription() {
            throw new NotImplementedException();
        }
    }
}