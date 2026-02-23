using MageFactory.Semantics;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
using UnityEngine.UI;

namespace MageFactory.UI.Component {
    public sealed class ItemViewPrefabItemView : Semantic<PlacedItemView> {
        public ItemViewPrefabItemView(PlacedItemView value) : base(value) {
        }
    }

    public sealed class DragGhostPrefabItemView : Semantic<PlacedItemView> {
        public DragGhostPrefabItemView(PlacedItemView value) : base(value) {
        }
    }

    public sealed class InventoryGridLayoutGroup : Semantic<GridLayoutGroup> {
        public InventoryGridLayoutGroup(GridLayoutGroup value) : base(value) {
        }
    }

    public sealed class GridViewPrefabInventoryGridView : Semantic<InventoryGridView> {
        public GridViewPrefabInventoryGridView(InventoryGridView value) : base(value) {
        }
    }

    public sealed class CellViewPrefabInventoryCellView : Semantic<InventoryCellView> {
        public CellViewPrefabInventoryCellView(InventoryCellView value) : base(value) {
        }
    }
}