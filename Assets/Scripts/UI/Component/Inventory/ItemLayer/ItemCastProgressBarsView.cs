using System.Collections.Generic;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public sealed class ItemCastProgressBarsView : MonoBehaviour {
        [Header("Visual")] [SerializeField] private Color trackColor = new(0.08f, 0.08f, 0.08f, 0.5f);
        [SerializeField] private Color fillColor = new(0.37f, 0.72f, 1f, 0.72f);

        private readonly List<ItemCellTileView> tileViews = new();

        public static ItemCastProgressBarsView create(Transform parent) {
            NullGuard.NotNullOrThrow(parent);

            var go = new GameObject(
                nameof(ItemCastProgressBarsView),
                typeof(RectTransform),
                typeof(ItemCastProgressBarsView));

            go.transform.SetParent(parent, false);

            var rectTransform = (RectTransform)go.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            return go.GetComponent<ItemCastProgressBarsView>();
        }

        public void bindTiles(IReadOnlyList<ItemCellTileView> tiles) {
            NullGuard.NotNullOrThrow(tiles);

            hideAll();
            tileViews.Clear();

            for (int i = 0; i < tiles.Count; i++) {
                tileViews.Add(NullGuard.NotNullOrThrow(tiles[i]));
            }
        }

        public void setProgressBars(IReadOnlyList<ItemCastProgressViewState> progressRatios) {
            NullGuard.NotNullOrThrow(progressRatios);

            if (progressRatios.Count == 0) {
                hideAll();
                return;
            }

            int tilesCount = tileViews.Count;
            for (int i = 0; i < tilesCount; i++) {
                tileViews[i].setCastProgressLanes(
                    progressRatios,
                    trackColor,
                    fillColor);
            }
        }

        public void hideAll() {
            for (int i = 0; i < tileViews.Count; i++) {
                tileViews[i].hideCastProgressLanes();
            }
        }
    }
}