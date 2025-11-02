using Inventory.Items.Config;
using Shared.Utility;

namespace Inventory.Items.Domain {

    /// <summary>
    ///     Logiczny model przedmiotu w ekwipunku.
    /// </summary>
    public class ShapeArchetype {
        public ShapeArchetype(ShapeArchetypeId shapeArchetypeId, string displayName, ItemShape shape, string iconId = null) {
            ShapeArchetypeId = shapeArchetypeId;
            DisplayName = displayName;
            Shape = shape;
            IconPath = iconId;
        }

        public ShapeArchetypeId ShapeArchetypeId { get; }
        public string DisplayName { get; }
        public ItemShape Shape { get; }
        public string IconPath { get; } // opcjonalnie
    }
}