using MageFactory.ActionEffect;
using MageFactory.Item.Catalog.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MageFactory.UI.Context.Combat.Feature.AddItem {
    public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [SerializeField] private ItemCatalogButtonKind itemKind = ItemCatalogButtonKind.RustySword;
        [SerializeField] private bool createCatalogButtonClones = true;

        private ItemDragService service;
        private IItemDefinition inventoryPlaceableItem;

        [Inject]
        public void construct(ItemDragService injectedService) {
            this.service = injectedService;
            createCatalogButtonsIfNeeded(injectedService);
            setButtonLabel(getButtonLabel(itemKind));
        }

        public void OnBeginDrag(PointerEventData eventData) {
            inventoryPlaceableItem = createItemDefinition(itemKind);
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

        private void createCatalogButtonsIfNeeded(ItemDragService injectedService) {
            if (!createCatalogButtonClones || transform.parent == null) {
                return;
            }

            if (transform.parent.Find("ItemButton_BasicOutputPort") != null) {
                return;
            }

            ItemCatalogButtonKind[] itemKinds = getButtonKinds();
            itemKind = itemKinds[0];
            setButtonLabel(getButtonLabel(itemKind));

            if (transform is RectTransform ownRect) {
                ownRect.anchoredPosition = new Vector2(0f, 72f);
            }

            for (int i = 1; i < itemKinds.Length; i++) {
                ItemCatalogButtonKind nextKind = itemKinds[i];
                GameObject itemButton = Instantiate(gameObject, transform.parent);
                itemButton.name = "ItemButton_" + nextKind;

                ItemDraggable draggable = itemButton.GetComponent<ItemDraggable>();
                draggable.itemKind = nextKind;
                draggable.createCatalogButtonClones = false;
                draggable.service = injectedService;
                draggable.setButtonLabel(getButtonLabel(nextKind));

                if (itemButton.transform is RectTransform rectTransform) {
                    rectTransform.anchoredPosition = new Vector2(0f, 72f - i * 28f);
                }
            }
        }

        private void setButtonLabel(string label) {
            TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
            if (buttonText == null) {
                return;
            }

            buttonText.text = label;
        }

        private static ItemCatalogButtonKind[] getButtonKinds() {
            return new[] {
                ItemCatalogButtonKind.RustySword,
                ItemCatalogButtonKind.Hammer,
                ItemCatalogButtonKind.EntryPointGem,
                ItemCatalogButtonKind.DefenseEntryPointGem,
                ItemCatalogButtonKind.Shield,
                ItemCatalogButtonKind.PulseInputPort,
                ItemCatalogButtonKind.BasicOutputPort
            };
        }

        private static IItemDefinition createItemDefinition(ItemCatalogButtonKind catalogButtonKind) {
            return catalogButtonKind switch {
                ItemCatalogButtonKind.RustySword => new RustySword(),
                ItemCatalogButtonKind.Hammer => new Hammer(),
                ItemCatalogButtonKind.EntryPointGem => new EntryPointGem(),
                ItemCatalogButtonKind.DefenseEntryPointGem => new DefenseEntryPointGem(),
                ItemCatalogButtonKind.Shield => new Shield(),
                ItemCatalogButtonKind.PulseInputPort => new PulseInputPort(),
                ItemCatalogButtonKind.BasicOutputPort => new BasicOutputPort(),
                _ => new RustySword()
            };
        }

        private static string getButtonLabel(ItemCatalogButtonKind catalogButtonKind) {
            return catalogButtonKind switch {
                ItemCatalogButtonKind.RustySword => "Sword",
                ItemCatalogButtonKind.Hammer => "Hammer",
                ItemCatalogButtonKind.EntryPointGem => "Entry",
                ItemCatalogButtonKind.DefenseEntryPointGem => "Defense Entry",
                ItemCatalogButtonKind.Shield => "Shield",
                ItemCatalogButtonKind.PulseInputPort => "IN Port",
                ItemCatalogButtonKind.BasicOutputPort => "OUT Port",
                _ => "Item"
            };
        }

        private enum ItemCatalogButtonKind {
            RustySword,
            Hammer,
            EntryPointGem,
            DefenseEntryPointGem,
            Shield,
            PulseInputPort,
            BasicOutputPort
        }
    }
}