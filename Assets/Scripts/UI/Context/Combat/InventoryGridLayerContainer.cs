using System;
using MageFactory.Shared.Model;
using UnityEngine;
using Zenject;

namespace MageFactory.Inventory.Controller {
    public interface ICombatInventoryPanel {
        public readonly struct UiChangeInventoryCommand {
            public readonly int width;
            public readonly int height;
            public readonly Func<Vector2Int, CellState> getState;

            public UiChangeInventoryCommand(int width, int height, Func<Vector2Int, CellState> getState) {
                this.width = width;
                this.height = height;
                this.getState = getState;
            }
        }

        public void changeInventory(UiChangeInventoryCommand changeInventoryCommand);
    }

    public class InventoryGridLayerContainer : MonoBehaviour, ICombatInventoryPanel {
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

        public void changeInventory(ICombatInventoryPanel.UiChangeInventoryCommand changeInventoryCommand) {
            instancedPrefabInventoryGridView.build(changeInventoryCommand);
        }
    }
}