using MageFactory.Item.Api.Dto;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Item.Domain {
    internal class ItemArchetype : IPlaceableItem {
        private readonly float castTime = 0.05f; // for now
        private readonly ShapeArchetype shapeArchetype;

        private ItemArchetype(ShapeArchetype shapeArchetype) {
            this.shapeArchetype = shapeArchetype;
        }

        internal static IPlaceableItem create(CreatePlaceableItemCommand createPlaceableItemCommand) {
            return new ItemArchetype(createPlaceableItemCommand.shapeArchetype);
        }

        public IPlacedItem toPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return new BattleItem(this, origin); // TODO
        }


        public ShapeArchetype getShape() {
            return shapeArchetype;
        }

        internal float getCastTime() {
            return castTime;
        }
    }
}