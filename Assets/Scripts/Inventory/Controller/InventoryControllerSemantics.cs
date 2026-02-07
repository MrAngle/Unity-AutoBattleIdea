using MageFactory.Inventory.Api;
using MageFactory.Semantics;
using UnityEngine.UI;

namespace MageFactory.Inventory.Controller {
    public sealed class ItemViewPrefabItemView : Semantic<ItemView> {
        public ItemViewPrefabItemView(ItemView value) : base(value) {
        }
    }

    public sealed class DragGhostPrefabItemView : Semantic<ItemView> {
        public DragGhostPrefabItemView(ItemView value) : base(value) {
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