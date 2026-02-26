using System;
using System.Collections.Generic;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component;
using MageFactory.UI.Component.Inventory;
using MageFactory.UI.Component.Inventory.ItemLayer;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Context.Combat.Feature.AddItem;
using Zenject;

namespace MageFactory.UI.Context.Combat {
    internal class CombatContextPresentationOrchestrator : ICombatCharacterCreatedEventListener,
        ICombatContextEventListener,
        IDisposable, IUiCombatCharacterSelectedEventListener, IItemPlacedEventEventListener, IHpChangedEventListener,
        ICharacterDeathEventListener {
        private ICombatContext combatContext; // TODO: change to "view model"
        private ICombatCharacter selectedCharacter;
        private readonly ICombatContextEventRegistry combatContextEventRegistry;
        private readonly IUiCombatContextEventRegistry uiCombatContextEventRegistry;
        private readonly IInventoryEventRegistry inventoryEventRegistry;
        private readonly InventoryPanelPresentation inventoryPanelPresentation;
        private readonly ItemDragService itemDragService;
        private readonly ICharacterEventRegistry characterEventRegistry;
        private readonly Dictionary<Id<CharacterId>, CharacterPrefabAggregate> characterPrefabs;

        [Inject]
        public CombatContextPresentationOrchestrator(ICombatContextEventRegistry combatContextEventRegistry,
                                                     IUiCombatContextEventRegistry uiCombatContextEventRegistry,
                                                     IInventoryEventRegistry inventoryEventRegistry,
                                                     ICharacterEventRegistry characterEventRegistry,
                                                     InventoryPanelPresentation inventoryPanelPresentation,
                                                     ItemDragService itemDragService,
                                                     Dictionary<Id<CharacterId>, CharacterPrefabAggregate>
                                                         characterPrefabs,
                                                     ICombatContext combatContext) {
            this.combatContextEventRegistry = NullGuard.NotNullOrThrow(combatContextEventRegistry);
            this.uiCombatContextEventRegistry = NullGuard.NotNullOrThrow(uiCombatContextEventRegistry);
            this.inventoryEventRegistry = NullGuard.NotNullOrThrow(inventoryEventRegistry);
            this.inventoryPanelPresentation = NullGuard.NotNullOrThrow(inventoryPanelPresentation);
            this.itemDragService = NullGuard.NotNullOrThrow(itemDragService);
            this.characterEventRegistry = NullGuard.NotNullOrThrow(characterEventRegistry);
            this.characterPrefabs = NullGuard.NotNullOrThrow(characterPrefabs);
            this.combatContext = NullGuard.NotNullOrThrow(combatContext);

            this.combatContextEventRegistry
                .subscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((ICombatContextEventListener)this);
            this.uiCombatContextEventRegistry
                .subscribe(this);
            this.characterEventRegistry
                .subscribe((IHpChangedEventListener)this);
            this.characterEventRegistry
                .subscribe((ICharacterDeathEventListener)this);
            this.inventoryEventRegistry.subscribe(this);
        }

        public static void create(ICombatContextEventRegistry combatContextEventRegistry,
                                  IUiCombatContextEventRegistry uiCombatContextEventRegistry,
                                  IInventoryEventRegistry inventoryEventRegistry,
                                  ICharacterEventRegistry characterEventRegistry,
                                  InventoryPanelPresentation inventoryPanelPresentation,
                                  ItemDragService itemDragService,
                                  Dictionary<Id<CharacterId>, CharacterPrefabAggregate> characterPrefabs,
                                  ICombatContext combatContext) {
            CombatContextPresentationOrchestrator combatContextPresentationOrchestrator =
                new CombatContextPresentationOrchestrator(
                    combatContextEventRegistry,
                    uiCombatContextEventRegistry,
                    inventoryEventRegistry,
                    characterEventRegistry,
                    inventoryPanelPresentation,
                    itemDragService,
                    characterPrefabs,
                    combatContext);
        }

        public void Dispose() {
            this.combatContextEventRegistry
                .unsubscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((ICombatContextEventListener)this);
            this.uiCombatContextEventRegistry
                .unsubscribe((IUiCombatCharacterSelectedEventListener)this);
            this.inventoryEventRegistry.unsubscribe(this);
            this.characterEventRegistry
                .unsubscribe((IHpChangedEventListener)this);
            this.characterEventRegistry
                .unsubscribe((ICharacterDeathEventListener)this);
        }

        public void onEvent(in CombatContextCreatedDtoEvent ev) {
            combatContext = ev.combatContext;

            onEvent(new UiCombatCharacterSelectedEvent(combatContext.getRandomCharacter().getId())); // for now
        }

        public void onEvent(in CombatCharacterCreatedDtoEvent ev) {
            // throw new System.NotImplementedException();
            // TODO: update view model
        }

        public void onEvent(in UiCombatCharacterSelectedEvent characterSelectedEvent) {
            selectedCharacter = combatContext.getCombatCharacterById(characterSelectedEvent.characterId);

            inventoryPanelPresentation.printInventory(selectedCharacter.getInventoryAggregate());

            itemDragService.setCharacterContext(selectedCharacter);
        }

        public void onEvent(in NewItemPlacedDtoEvent ev) {
            ICombatInventoryItemsPanel.NewItemPrintCommand itemPrintCommand =
                new(ev.placedItemId, ev.shapeArchetype, ev.origin);
            inventoryPanelPresentation.printNewItem(itemPrintCommand);
        }

        public void onEvent(in CharacterHpChangedDtoEvent ev) {
            if (characterPrefabs.TryGetValue(ev.characterId, out var prefab)) {
                prefab.hpChange(ev);
            }
        }

        public void onEvent(in CharacterDeathDtoEvent ev) {
            if (characterPrefabs.TryGetValue(ev.characterId, out var prefab)) {
                prefab.destroy(ev);
            }
        }
    }
}