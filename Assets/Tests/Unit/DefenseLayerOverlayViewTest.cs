using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
using NUnit.Framework;
using UnityEngine;

namespace MageFactory.Tests.Unit {
    public sealed class DefenseLayerOverlayViewTest {
        private readonly List<GameObject> createdObjects = new();

        [TearDown]
        public void tearDown() {
            for (int i = 0; i < createdObjects.Count; i++) {
                Object.DestroyImmediate(createdObjects[i]);
            }

            createdObjects.Clear();
        }

        [Test]
        public void should_position_stability_above_guard_and_hp_below_guard() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);
            PreparedGuardOverlayView guardOverlayView = PreparedGuardOverlayView.create(parent.transform);
            var guardId = new Id<GuardId>(1);
            var gridInfo = new ICombatInventoryGridPanel.InventoryGridInfo(
                4,
                4,
                new Vector2(40f, 40f),
                new Vector2(4f, 4f),
                Vector2.zero);

            defenseLayerOverlayView.printDefenseLayers(
                StabilityMitigationCalculator.ReferenceStability,
                StabilityMitigationCalculator.ReferenceStability,
                90,
                100,
                gridInfo);
            guardOverlayView.printGuards(
                new[] {
                    new PreparedGuardState(guardId, new GuardPower(5))
                },
                gridInfo);

            Assert.That(defenseLayerOverlayView.tryGetStabilityCenterInParent(out Vector2 stabilityCenter), Is.True);
            Assert.That(guardOverlayView.tryGetGuardCenterInParent(guardId, out Vector2 guardCenter), Is.True);

            Vector2 hpCenter = getDefenseLayerChildCenter(defenseLayerOverlayView, "HpRow");

            Assert.That(stabilityCenter.y, Is.GreaterThan(guardCenter.y));
            Assert.That(hpCenter.y, Is.LessThan(guardCenter.y));
        }

        private GameObject createRectGameObject(string name) {
            var go = new GameObject(name, typeof(RectTransform));
            createdObjects.Add(go);
            return go;
        }

        private static Vector2 getDefenseLayerChildCenter(DefenseLayerOverlayView overlayView, string childName) {
            RectTransform overlayRectTransform = (RectTransform)overlayView.transform;
            RectTransform childRectTransform = (RectTransform)overlayView.transform.Find(childName);

            return overlayRectTransform.anchoredPosition
                   + childRectTransform.anchoredPosition
                   + new Vector2(
                       childRectTransform.rect.width * 0.5f,
                       -childRectTransform.rect.height * 0.5f);
        }
    }
}