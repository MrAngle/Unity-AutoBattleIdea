using System.Collections.Generic;
using System.Reflection;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.UI.Component.Inventory.GridLayer;
using MageFactory.UI.Component.Inventory.ItemLayer;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

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

        [Test]
        public void should_fill_stability_bar_by_current_to_baseline_ratio() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);

            defenseLayerOverlayView.printDefenseLayers(
                50,
                100,
                100,
                100,
                createGridInfo());

            Assert.That(
                getDefenseLayerFillAmount(defenseLayerOverlayView, "StabilityRow"),
                Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(
                getDefenseLayerFillAnchor(defenseLayerOverlayView, "StabilityRow"),
                Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void should_fill_hp_bar_by_current_to_max_hp_ratio() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);

            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                50,
                100,
                createGridInfo());

            Assert.That(
                getDefenseLayerFillAmount(defenseLayerOverlayView, "HpRow"),
                Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(
                getDefenseLayerFillRatioInsideFrame(defenseLayerOverlayView, "HpRow"),
                Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void should_show_stability_value_and_damage_reduction_labels() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);

            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                100,
                100,
                createGridInfo());

            Assert.That(getText(defenseLayerOverlayView, "StabilityRow/Value"), Is.EqualTo("100 / 100"));
            Assert.That(
                getText(defenseLayerOverlayView, "StabilityRow/Detail"),
                Is.EqualTo("Damage Reduction: 50%"));
            Assert.That(getText(defenseLayerOverlayView, "StabilityRow/Extra"), Does.Contain("DecayAt: 110"));
        }

        [Test]
        public void should_show_hp_current_to_max_value_on_hp_bar() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);

            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                50,
                100,
                createGridInfo());

            Assert.That(getText(defenseLayerOverlayView, "HpRow/Value"), Is.EqualTo("50 / 100"));
        }

        [Test]
        public void should_show_damage_packet_as_single_layer_value_without_debug_arrow_text() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);
            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                100,
                100,
                createGridInfo());

            defenseLayerOverlayView.showDamagePacketLayer(7, 0, 100, false);
            string valueText = getDamagePacketValueText(defenseLayerOverlayView, 0);

            Assert.That(valueText, Is.EqualTo("100"));

            defenseLayerOverlayView.showDamagePacketLayer(7, 1, 50, false);

            valueText = getDamagePacketValueText(defenseLayerOverlayView, 0);
            Assert.That(valueText, Is.EqualTo("50"));
            Assert.That(valueText, Does.Not.Contain(">"));
            Assert.That(defenseLayerOverlayView.transform.Find("DamagePacketPath/Connector"), Is.Null);
        }

        [Test]
        public void should_keep_latest_five_damage_packet_values_visible_with_overflow_badge() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);
            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                100,
                100,
                createGridInfo());

            for (int i = 0; i < 6; i++) {
                defenseLayerOverlayView.showDamagePacketLayer(i + 1, 0, 100 + i, false);
            }

            for (int i = 0; i < 5; i++) {
                Assert.That(getDamagePacketValueObject(defenseLayerOverlayView, i).activeSelf, Is.True);
            }

            Assert.That(getVisibleDamagePacketValues(defenseLayerOverlayView), Is.EquivalentTo(new[] {
                "101", "102", "103", "104", "105"
            }));
            Assert.That(getText(defenseLayerOverlayView, "DamagePacketPath/Overflow"), Is.EqualTo("5+"));
            Assert.That(
                defenseLayerOverlayView.transform.Find("DamagePacketPath/Overflow").gameObject.activeSelf,
                Is.True);
        }

        [Test]
        public void should_keep_incoming_packet_visible_until_resolved_layer_continues_it() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);
            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                100,
                100,
                createGridInfo());

            defenseLayerOverlayView.showDamagePacketLayer(10, 0, 5, false);
            defenseLayerOverlayView.showDamagePacketLayer(10, 0, 5, true);

            Assert.That(getDamagePacketValueObject(defenseLayerOverlayView, 0).activeSelf, Is.True);
            Assert.That(getVisibleDamagePacketValues(defenseLayerOverlayView), Is.EqualTo(new[] { "5" }));

            defenseLayerOverlayView.showDamagePacketLayer(11, 1, 3, false);

            Assert.That(getVisibleDamagePacketValues(defenseLayerOverlayView), Is.EqualTo(new[] { "3" }));
        }

        [Test]
        public void should_render_damage_packets_above_defense_layer_components() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);

            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                100,
                100,
                createGridInfo());

            Assert.That(
                defenseLayerOverlayView.transform.Find("DamagePacketPath").GetSiblingIndex(),
                Is.GreaterThan(defenseLayerOverlayView.transform.Find("HpRow").GetSiblingIndex()));
        }

        [Test]
        public void should_layout_damage_packets_in_left_component_and_layers_to_the_right() {
            GameObject parent = createRectGameObject("ItemsLayer");
            DefenseLayerOverlayView defenseLayerOverlayView = DefenseLayerOverlayView.create(parent.transform);
            PreparedGuardOverlayView guardOverlayView = PreparedGuardOverlayView.create(parent.transform);
            ICombatInventoryGridPanel.InventoryGridInfo gridInfo = createGridInfo();

            defenseLayerOverlayView.printDefenseLayers(
                100,
                100,
                100,
                100,
                gridInfo);
            guardOverlayView.printGuards(new PreparedGuardState[0], gridInfo);

            RectTransform packetPath = getRectTransform(defenseLayerOverlayView, "DamagePacketPath");
            RectTransform stabilityRow = getRectTransform(defenseLayerOverlayView, "StabilityRow");
            RectTransform hpRow = getRectTransform(defenseLayerOverlayView, "HpRow");
            RectTransform guardRoot = (RectTransform)guardOverlayView.transform;
            float expectedOverlayWidth = Mathf.Max(
                DefenseLayerOverlayView.LayerContentOffsetX + 420f,
                calculateInventoryWidth(gridInfo));
            float expectedLayerWidth = DefenseLayerOverlayView.calculateLayerContentWidth(expectedOverlayWidth);

            Assert.That(packetPath.gameObject.activeSelf, Is.True);
            Assert.That(packetPath.anchoredPosition.x, Is.EqualTo(0f).Within(0.001f));
            Assert.That(packetPath.rect.width, Is.EqualTo(DefenseLayerOverlayView.PacketLaneWidth).Within(0.001f));
            Assert.That(
                packetPath.rect.width,
                Is.GreaterThanOrEqualTo(DefenseLayerOverlayView.VisibleDamagePacketCapacity
                                        * DefenseLayerOverlayView.DamagePacketValueWidth
                                        + (DefenseLayerOverlayView.VisibleDamagePacketCapacity - 1)
                                        * DefenseLayerOverlayView.DamagePacketValueGap));
            Assert.That(stabilityRow.anchoredPosition.x,
                Is.EqualTo(DefenseLayerOverlayView.LayerContentOffsetX).Within(0.001f));
            Assert.That(hpRow.anchoredPosition.x,
                Is.EqualTo(DefenseLayerOverlayView.LayerContentOffsetX).Within(0.001f));
            Assert.That(guardRoot.anchoredPosition.x, Is.EqualTo(
                DefenseLayerOverlayView.RootPadding + DefenseLayerOverlayView.LayerContentOffsetX).Within(0.001f));
            Assert.That(stabilityRow.rect.width, Is.EqualTo(expectedLayerWidth).Within(0.001f));
            Assert.That(hpRow.rect.width, Is.EqualTo(expectedLayerWidth).Within(0.001f));
            Assert.That(guardRoot.rect.width, Is.EqualTo(expectedLayerWidth).Within(0.001f));
        }

        [Test]
        public void should_keep_guard_layer_visible_without_prepared_guards() {
            GameObject parent = createRectGameObject("ItemsLayer");
            PreparedGuardOverlayView guardOverlayView = PreparedGuardOverlayView.create(parent.transform);

            guardOverlayView.printGuards(new PreparedGuardState[0], createGridInfo());

            Assert.That(guardOverlayView.gameObject.activeSelf, Is.True);
            Assert.That(guardOverlayView.transform.Find("GuardLayerFrame"), Is.Not.Null);
        }

        private GameObject createRectGameObject(string name) {
            var go = new GameObject(name, typeof(RectTransform));
            createdObjects.Add(go);
            return go;
        }

        private static ICombatInventoryGridPanel.InventoryGridInfo createGridInfo() {
            return new ICombatInventoryGridPanel.InventoryGridInfo(
                4,
                4,
                new Vector2(40f, 40f),
                new Vector2(4f, 4f),
                Vector2.zero);
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

        private static RectTransform getRectTransform(DefenseLayerOverlayView overlayView, string childName) {
            return (RectTransform)overlayView.transform.Find(childName);
        }

        private static float calculateInventoryWidth(ICombatInventoryGridPanel.InventoryGridInfo inventoryGridInfo) {
            int widthCells = Mathf.Max(1, inventoryGridInfo.WidthCellsNumber);
            return widthCells * inventoryGridInfo.CellSize.x
                   + Mathf.Max(0, widthCells - 1) * inventoryGridInfo.Spacing.x;
        }

        private static float getDefenseLayerFillAmount(DefenseLayerOverlayView overlayView, string childName) {
            Image fillImage = overlayView.transform.Find(childName + "/Fill").GetComponent<Image>();
            return fillImage.fillAmount;
        }

        private static float getDefenseLayerFillAnchor(DefenseLayerOverlayView overlayView, string childName) {
            RectTransform fillRectTransform = (RectTransform)overlayView.transform.Find(childName + "/Fill");
            return fillRectTransform.anchorMax.x;
        }

        private static float
            getDefenseLayerFillRatioInsideFrame(DefenseLayerOverlayView overlayView, string childName) {
            RectTransform fillRectTransform = (RectTransform)overlayView.transform.Find(childName + "/Fill");
            RectTransform frameRectTransform = (RectTransform)overlayView.transform.Find(childName + "/FillFrame");
            float frameWidth = frameRectTransform.anchorMax.x - frameRectTransform.anchorMin.x;
            if (frameWidth <= 0f) {
                return 0f;
            }

            return (fillRectTransform.anchorMax.x - frameRectTransform.anchorMin.x) / frameWidth;
        }

        private static string getDamagePacketValueText(DefenseLayerOverlayView overlayView, int packetSlotIndex) {
            return getText(overlayView, "DamagePacketPath/PacketValue" + packetSlotIndex + "/Value");
        }

        private static GameObject getDamagePacketValueObject(
            DefenseLayerOverlayView overlayView,
            int packetSlotIndex) {
            return overlayView.transform.Find("DamagePacketPath/PacketValue" + packetSlotIndex).gameObject;
        }

        private static IReadOnlyList<string> getVisibleDamagePacketValues(DefenseLayerOverlayView overlayView) {
            var values = new List<string>();
            for (int i = 0; i < 5; i++) {
                GameObject packetObject = getDamagePacketValueObject(overlayView, i);
                if (!packetObject.activeSelf) {
                    continue;
                }

                values.Add(getDamagePacketValueText(overlayView, i));
            }

            return values;
        }

        private static string getText(DefenseLayerOverlayView overlayView, string textPath) {
            Component textComponent = overlayView.transform.Find(textPath)
                .GetComponent("TextMeshProUGUI");
            PropertyInfo textProperty = textComponent.GetType().GetProperty("text");
            return (string)textProperty.GetValue(textComponent);
        }
    }
}