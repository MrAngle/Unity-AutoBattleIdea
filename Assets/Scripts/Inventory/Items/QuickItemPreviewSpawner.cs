using Inventory.Items;
using UI.Inventory.Items.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Items {


    public class QuickItemPreviewSpawner : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform itemsLayer;     // wskaż ItemsLayer (dziecko InventoryPanel)
        [SerializeField] private GridLayoutGroup gridLayout;   // wskaż GridLayoutGroup siatki
        [SerializeField] private ItemView itemViewPrefab;      // wskaż ItemViewPrefab

        [Header("Where to show")]
        [SerializeField] private Vector2Int origin = new(1, 1); // w jakiej komórce postawić

        private void Start()
        {
            var cellSize = gridLayout.cellSize;
            var spacingX = gridLayout.spacing.x;

            // 1×1 kwadrat – jeśli masz ItemConfig, możesz użyć np. ItemConfig.GemShard
            var data = new ItemData(
                id: "demo_square",
                displayName: "Demo Square",
                shape: ItemShape.SingleCell()
            );

            var view = Instantiate(itemViewPrefab, itemsLayer, false);
            view.Build(data, cellSize);
            view.SetOriginInGrid(origin, cellSize, gridOrigin: Vector2.zero, spacing: spacingX);
        }
    }

}