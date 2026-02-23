using System;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Model;
using UnityEngine;
using Zenject;

namespace MageFactory.UI.Component.Inventory.GridLayer {
    public interface ICombatInventoryGridPanel {
        public void printInventoryGrid(UiPrintInventoryGridCommand printInventoryGridCommand);

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
                    characterInventory.Width,
                    characterInventory.Height,
                    coord => characterInventory.getState(coord));
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

            var rt = (RectTransform)instancedPrefabInventoryGridView.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public void printInventoryGrid(
            ICombatInventoryGridPanel.UiPrintInventoryGridCommand printInventoryGridCommand) {
            instancedPrefabInventoryGridView.build(printInventoryGridCommand);
        }
    }
}