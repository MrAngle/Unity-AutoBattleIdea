using System.Collections.Generic;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.GridLayer {
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

        public void build(ICombatInventoryGridPanel.UiPrintInventoryGridCommand printInventoryGridCommand) {
            Debug.Log($"[Grid] Parent: {transform.name}, after build children: {transform.childCount}");
            clear();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = printInventoryGridCommand.width;

            for (var y = 0; y < printInventoryGridCommand.height; y++)
            for (var x = 0; x < printInventoryGridCommand.width; x++) {
                var coord = new Vector2Int(x, y);
                var v = Instantiate(cellPrefab.Get(), gridLayout.transform);
                v.Init(coord, printInventoryGridCommand.getState(coord));
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