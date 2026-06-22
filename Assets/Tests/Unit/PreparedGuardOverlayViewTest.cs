using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit {
    public sealed class PreparedGuardOverlayViewTest {
        private readonly List<GameObject> createdObjects = new();

        [TearDown]
        public void tearDown() {
            for (int i = 0; i < createdObjects.Count; i++) {
                Object.DestroyImmediate(createdObjects[i]);
            }

            createdObjects.Clear();
        }

        [Test]
        public void should_position_guard_icons_above_inventory_layer() {
            GameObject parent = createRectGameObject("ItemsLayer");
            PreparedGuardOverlayView overlayView = PreparedGuardOverlayView.create(parent.transform);
            var guardId = new Id<GuardId>(1);

            overlayView.printGuards(
                new[] {
                    new PreparedGuardState(guardId, new GuardPower(5))
                },
                new ICombatInventoryGridPanel.InventoryGridInfo(
                    4,
                    4,
                    new Vector2(40f, 40f),
                    new Vector2(4f, 4f),
                    Vector2.zero));

            RectTransform overlayRectTransform = (RectTransform)overlayView.transform;

            Assert.That(overlayRectTransform.anchoredPosition.y, Is.GreaterThan(0f));
            Assert.That(overlayView.tryGetGuardCenterInParent(guardId, out Vector2 guardCenter), Is.True);
            Assert.That(guardCenter.y, Is.GreaterThan(0f));
        }

        private GameObject createRectGameObject(string name) {
            var go = new GameObject(name, typeof(RectTransform));
            createdObjects.Add(go);
            return go;
        }
    }
}