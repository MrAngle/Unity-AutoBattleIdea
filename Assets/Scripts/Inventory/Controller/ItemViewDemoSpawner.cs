using MageFactory.Inventory.Api;
using MageFactory.Shared.Model.Shape;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.Inventory.Controller {
    public class ItemViewDemoSpawner : MonoBehaviour {
        [Header("Refs")] [SerializeField] private RectTransform itemsLayer; // wskaż ItemsLayer

        [SerializeField] private GridLayoutGroup gridLayout; // wskaż GridLayoutGroup z Twojego grida
        private ItemView itemViewPrefab; // wskaż ItemViewPrefab

        private void Reset() {
            if (!itemsLayer) {
                var t = transform.Find("ItemsLayer");
                if (t) itemsLayer = (RectTransform)t;
            }

            if (!gridLayout)
                gridLayout = GetComponentInChildren<GridLayoutGroup>();
        }

        private void Start() {
            // Jeśli nie przypisane w Inspectorze, spróbuj znaleźć:
            if (!gridLayout) gridLayout = GetComponentInChildren<GridLayoutGroup>(true);

            var cellSize = gridLayout.cellSize;
            var spacing = gridLayout.spacing.x;

            SpawnAt(ShapeCatalog.Square1x1, new Vector2Int(1, 1), cellSize, spacing);
            SpawnAt(ShapeCatalog.Square2x2, new Vector2Int(4, 1), cellSize, spacing);
            SpawnAt(ShapeCatalog.LBracket, new Vector2Int(1, 4), cellSize, spacing);
        }

        private void SpawnAt(ShapeArchetype data, Vector2Int origin, Vector2 cellSize, float spacing) {
            var view = Instantiate(itemViewPrefab, itemsLayer, false);
            view.Build(data, cellSize);

            // Zakładamy, że lewy-górny rogu grida w anchored to (0,0).
            // Jeśli Twój grid ma offset, przekaż go zamiast Vector2.zero.
            view.SetOriginInGrid(origin, cellSize, Vector2.zero, spacing);
        }
    }
}