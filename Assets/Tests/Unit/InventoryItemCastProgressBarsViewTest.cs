using System.Collections.Generic;
using MageFactory.Shared.Id;
using MageFactory.UI.Component.Inventory.ItemLayer;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.Tests.Unit {
    public sealed class InventoryItemCastProgressBarsViewTest {
        private readonly List<GameObject> createdObjects = new();

        [TearDown]
        public void tearDown() {
            for (int i = 0; i < createdObjects.Count; i++) {
                Object.DestroyImmediate(createdObjects[i]);
            }

            createdObjects.Clear();
        }

        [Test]
        public void should_enable_update_only_when_progress_is_visible() {
            GameObject parent = createRectGameObject("Item");
            ItemCellTileView tile = createTile(parent.transform, 0);
            ItemCastProgressBarsView progressBarsView = ItemCastProgressBarsView.create(parent.transform);
            progressBarsView.bindTiles(new[] { tile });

            Assert.That(progressBarsView.enabled, Is.False);

            progressBarsView.setProgressBars(new[] { createProgress(1, 0.5f) });

            Assert.That(progressBarsView.enabled, Is.True);

            progressBarsView.hideAll();

            Assert.That(progressBarsView.enabled, Is.False);
        }

        [Test]
        public void should_resize_active_lane_to_full_height_after_previous_overlap_disappears() {
            GameObject parent = createRectGameObject("Item");
            ItemCellTileView tile = createTile(parent.transform, 0);

            tile.setCastProgressLanes(
                new[] {
                    createProgress(1, 0.3f),
                    createProgress(2, 0.7f)
                },
                Color.black);

            RectTransform firstLane = getLane(tile, 0);
            RectTransform secondLane = getLane(tile, 1);

            Assert.That(firstLane.anchorMin.y, Is.EqualTo(0f).Within(0.001f));
            Assert.That(firstLane.anchorMax.y, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(secondLane.anchorMin.y, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(secondLane.anchorMax.y, Is.EqualTo(1f).Within(0.001f));

            tile.setCastProgressLanes(
                new[] { createProgress(1, 0.8f) },
                Color.black);

            Assert.That(firstLane.anchorMin.y, Is.EqualTo(0f).Within(0.001f));
            Assert.That(firstLane.anchorMax.y, Is.EqualTo(1f).Within(0.001f));
            Assert.That(secondLane.gameObject.activeSelf, Is.False);
        }

        private GameObject createRectGameObject(string name) {
            var go = new GameObject(name, typeof(RectTransform));
            createdObjects.Add(go);
            return go;
        }

        private ItemCellTileView createTile(Transform parent, int localRow) {
            GameObject go = createRectGameObject($"Tile_{localRow}");
            go.AddComponent<Image>();
            ItemCellTileView tile = go.AddComponent<ItemCellTileView>();
            go.transform.SetParent(parent, false);
            tile.bindShapeCell(new Vector2Int(0, localRow), 0, 1);
            tile.setupVisual(Color.white);
            return tile;
        }

        private static ItemCastProgressViewState createProgress(long flowId, float progressRatio) {
            return new ItemCastProgressViewState(
                new Id<ActiveFlowId>(flowId),
                0,
                progressRatio,
                0);
        }

        private static RectTransform getLane(ItemCellTileView tile, int index) {
            Transform lane = tile.transform.Find($"CastProgressLane_{index}");
            Assert.That(lane, Is.Not.Null);
            return (RectTransform)lane;
        }
    }
}