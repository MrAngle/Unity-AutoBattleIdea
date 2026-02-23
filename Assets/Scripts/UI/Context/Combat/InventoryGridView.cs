using System.Collections.Generic;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.Inventory.Controller {
    public class InventoryGridView : MonoBehaviour {
        private readonly Dictionary<Vector2Int, InventoryCellView> inventoryCellViews = new();
        private CellViewPrefabInventoryCellView cellPrefab;
        private GridLayoutGroup gridLayout;

        private void Awake() {
            // gwarancja, że zawsze bierzemy GridLayoutGroup z tego konkretnego prefab GO
            gridLayout = GetComponent<GridLayoutGroup>();
        }

        internal static InventoryGridView create(GridViewPrefabInventoryGridView prefab,
                                                 Transform parentTransform,
                                                 CellViewPrefabInventoryCellView cellPrefab) {
            NullGuard.NotNullOrThrow(prefab);
            NullGuard.NotNullOrThrow(parentTransform);
            NullGuard.NotNullOrThrow(cellPrefab);

            var gridView = NullGuard.NotNullOrThrow(
                Instantiate(prefab.Get(), parentTransform, false)
            );
            gridView.transform.SetParent(parentTransform, false);
            gridView.setCellPrefab(cellPrefab);

            return gridView;
        }

        public void build(ICombatInventoryPanel.UiChangeInventoryCommand changeInventoryCommand) {
            Debug.Log($"[Grid] Parent: {transform.name}, after build children: {transform.childCount}");
            clear();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = changeInventoryCommand.width;

            for (var y = 0; y < changeInventoryCommand.height; y++)
            for (var x = 0; x < changeInventoryCommand.width; x++) {
                var coord = new Vector2Int(x, y);
                var v = Instantiate(cellPrefab.Get(), gridLayout.transform);
                v.Init(coord, changeInventoryCommand.getState(coord));
                inventoryCellViews[coord] = v;
            }
        }

        public void clear() {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            inventoryCellViews.Clear();
        }

        private void setCellPrefab(CellViewPrefabInventoryCellView prefab) {
            cellPrefab = NullGuard.NotNullOrThrow(prefab);
        }
    }
}