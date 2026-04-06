using System.Collections.Generic;
using MageFactory.Shared.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.UI.Component.Inventory.GridLayer {
    public class InventoryGridView : MonoBehaviour {
        private readonly Dictionary<Vector2Int, InventoryCellView> inventoryCellViews = new();

        private CellViewPrefabInventoryCellView cellPrefab;
        private GridLayoutGroup gridLayout;

        private int widthCellsNumber;
        private int heightCellsNumber;

        private void Awake() {
            gridLayout = GetComponent<GridLayoutGroup>();
        }

        internal static InventoryGridView create(
            GridViewPrefabInventoryGridView prefab,
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

            widthCellsNumber = printInventoryGridCommand.width;
            heightCellsNumber = printInventoryGridCommand.height;

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = widthCellsNumber;

            for (var y = 0; y < heightCellsNumber; y++) {
                for (var x = 0; x < widthCellsNumber; x++) {
                    var coord = new Vector2Int(x, y);
                    InventoryCellView inventoryCellView = Instantiate(cellPrefab.Get(), gridLayout.transform);
                    inventoryCellView.Init(coord, printInventoryGridCommand.getState(coord));
                    inventoryCellViews[coord] = inventoryCellView;
                }
            }
        }

        public void clear() {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            inventoryCellViews.Clear();
            widthCellsNumber = 0;
            heightCellsNumber = 0;
        }

        public int getWidthCellsNumber() {
            return widthCellsNumber;
        }

        public int getHeightCellsNumber() {
            return heightCellsNumber;
        }

        public Vector2 getCellSize() {
            return gridLayout.cellSize;
        }

        public Vector2 getSpacing() {
            return gridLayout.spacing;
        }

        public Vector2 getGridOrigin() {
            RectTransform rectTransform = (RectTransform)gridLayout.transform;
            return rectTransform.anchoredPosition;
        }

        private void setCellPrefab(CellViewPrefabInventoryCellView prefab) {
            cellPrefab = NullGuard.NotNullOrThrow(prefab);
        }
    }
}