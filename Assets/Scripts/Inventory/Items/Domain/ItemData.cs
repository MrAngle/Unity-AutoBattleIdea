using Inventory.Items.Config;
using Shared.Utility;

namespace Inventory.Items.Domain {

    /// <summary>
    ///     Logiczny model przedmiotu w ekwipunku.
    /// </summary>
    public class ItemData {
        public ItemData(ItemDataId itemDataId, string displayName, ItemShape shape, string iconId = null) {
            ItemDataId = itemDataId;
            DisplayName = displayName;
            Shape = shape;
            IconPath = iconId;
        }

        public ItemDataId ItemDataId { get; }
        public string DisplayName { get; }
        public ItemShape Shape { get; }
        public string IconPath { get; } // opcjonalnie
    }
}