using System;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;
using UnityEngine;
using Zenject;

namespace MageFactory.UI.Component.Inventory.GridLayer {
    public interface ICombatInventoryGridPanel {
        void printInventoryGrid(UiPrintInventoryGridCommand printInventoryGridCommand);
        InventoryGridInfo getInventoryGridInfo();

        public readonly struct UiPrintInventoryGridCommand {
            public readonly int width;
            public readonly int height;
            public readonly Func<Vector2Int, CellState> getState;

            public UiPrintInventoryGridCommand(int width, int height, Func<Vector2Int, CellState> getState) {
                this.width = width;
                this.height = height;
                this.getState = getState;
            }

            public static UiPrintInventoryGridCommand from(ICombatInventory characterInventory) {
                return new UiPrintInventoryGridCommand(
                    characterInventory.getWidthCellsNumber(),
                    characterInventory.getHeightCellsNumber(),
                    coord => characterInventory.getState(coord));
            }
        }

        public readonly struct InventoryGridInfo {
            public readonly int WidthCellsNumber;
            public readonly int HeightCellsNumber;
            public readonly Vector2 CellSize;
            public readonly Vector2 Spacing;
            public readonly Vector2 GridOrigin;

            public InventoryGridInfo(
                int widthCellsNumber,
                int heightCellsNumber,
                Vector2 cellSize,
                Vector2 spacing,
                Vector2 gridOrigin) {
                WidthCellsNumber = widthCellsNumber;
                HeightCellsNumber = heightCellsNumber;
                CellSize = cellSize;
                Spacing = spacing;
                GridOrigin = gridOrigin;
            }
        }
    }

    public class InventoryGridLayerContainer : MonoBehaviour, ICombatInventoryGridPanel {
        private InventoryGridView instancedPrefabInventoryGridView;

        [Inject]
        private void construct(
            GridViewPrefabInventoryGridView gridViewPrefabInventoryGridView,
            CellViewPrefabInventoryCellView cellViewPrefabInventoryCellView
        ) {
            instancedPrefabInventoryGridView = InventoryGridView.create(gridViewPrefabInventoryGridView,
                transform, cellViewPrefabInventoryCellView);

            var rectTransform = (RectTransform)instancedPrefabInventoryGridView.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public void printInventoryGrid(
            ICombatInventoryGridPanel.UiPrintInventoryGridCommand printInventoryGridCommand) {
            instancedPrefabInventoryGridView.build(printInventoryGridCommand);
        }

        public ICombatInventoryGridPanel.InventoryGridInfo getInventoryGridInfo() {
            return new ICombatInventoryGridPanel.InventoryGridInfo(
                instancedPrefabInventoryGridView.getWidthCellsNumber(),
                instancedPrefabInventoryGridView.getHeightCellsNumber(),
                instancedPrefabInventoryGridView.getCellSize(),
                instancedPrefabInventoryGridView.getSpacing(),
                instancedPrefabInventoryGridView.getGridOrigin()
            );
        }
    }
}