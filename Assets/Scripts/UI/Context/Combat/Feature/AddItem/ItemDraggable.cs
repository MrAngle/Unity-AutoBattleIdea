using MageFactory.ActionEffect;
using MageFactory.Item.Catalog.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.UI.Context.Combat.Feature.AddItem {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [SerializeField] private ItemDragPoolKind dragPoolKind = ItemDragPoolKind.Offensive;
        [SerializeField] private bool createDefensiveButtonClone = true;

        private ItemDragService service;
        private IItemDefinition inventoryPlaceableItem;

        [Inject]
        public void construct(ItemDragService injectedService) {
            this.service = injectedService;
            createDefensiveButtonIfNeeded(injectedService);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            inventoryPlaceableItem = getRandomItemDefinition();
            if (inventoryPlaceableItem == null) return;
            service?.beginDrag(inventoryPlaceableItem, eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (inventoryPlaceableItem == null) return;
            service?.updateDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (inventoryPlaceableItem == null) return;
            service?.endDrag(eventData);
            inventoryPlaceableItem = null;
        }

        private IItemDefinition getRandomItemDefinition() {
            return dragPoolKind == ItemDragPoolKind.Defensive
                ? getRandomDefensiveItemDefinition()
                : getRandomOffensiveItemDefinition();
        }

        private static IItemDefinition getRandomOffensiveItemDefinition() {
            int roll = Random.Range(0, 100);

            if (roll < 10) {
                return new Hammer();
            }

            if (roll < 40) {
                return new EntryPointGem();
            }

            return new RustySword();
        }

        private static IItemDefinition getRandomDefensiveItemDefinition() {
            return Random.Range(0, 2) == 0
                ? new DefenseEntryPointGem()
                : new Shield();
        }

        private void createDefensiveButtonIfNeeded(ItemDragService injectedService) {
            if (!createDefensiveButtonClone || dragPoolKind != ItemDragPoolKind.Offensive || transform.parent == null) {
                return;
            }

            if (transform.parent.Find("DefensiveItemButton") != null) {
                return;
            }

            setButtonLabel("Offense");

            GameObject defensiveButton = Instantiate(gameObject, transform.parent);
            defensiveButton.name = "DefensiveItemButton";

            ItemDraggable defensiveDraggable = defensiveButton.GetComponent<ItemDraggable>();
            defensiveDraggable.dragPoolKind = ItemDragPoolKind.Defensive;
            defensiveDraggable.createDefensiveButtonClone = false;
            defensiveDraggable.service = injectedService;
            defensiveDraggable.setButtonLabel("Defense");

            if (transform is RectTransform offensiveRect && defensiveButton.transform is RectTransform defensiveRect) {
                offensiveRect.anchoredPosition = new Vector2(0, 18);
                defensiveRect.anchoredPosition = new Vector2(0, -18);
            }
        }

        private void setButtonLabel(string label) {
            TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
            if (buttonText == null) {
                return;
            }

            buttonText.text = label;
        }

        private enum ItemDragPoolKind {
            Offensive,
            Defensive
        }
    }
}