using System;
using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Api.Event;
using MageFactory.Inventory.Api.Event.Dto;
using MageFactory.Inventory.Controller;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Context.Combat.Inventory;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.UI.Context.Combat {
    internal class CombatContextPresentationHandler : ICombatCharacterCreatedEventListener, ICombatContextEventListener,
        IDisposable, IUiCombatCharacterSelectedEventListener, IItemPlacedEventEventListener {
        private ICombatContext combatContext; // TODO: change to "view model"
        private ICombatCharacter selectedCharacter;
        private readonly ICombatContextEventRegistry combatContextEventRegistry;
        private readonly IUiCombatContextEventRegistry uiCombatContextEventRegistry;
        private readonly IInventoryEventRegistry inventoryEventRegistry;
        private readonly InventoryPanelPresentation inventoryPanelPresentation;
        private readonly ItemDragService itemDragService;

        [Inject]
        public CombatContextPresentationHandler(ICombatContextEventRegistry combatContextEventRegistry,
                                                IUiCombatContextEventRegistry uiCombatContextEventRegistry,
                                                IInventoryEventRegistry inventoryEventRegistry,
                                                InventoryPanelPresentation inventoryPanelPresentation,
                                                ItemDragService itemDragService) {
            this.combatContextEventRegistry = NullGuard.NotNullOrThrow(combatContextEventRegistry);
            this.uiCombatContextEventRegistry = NullGuard.NotNullOrThrow(uiCombatContextEventRegistry);
            this.inventoryEventRegistry = NullGuard.NotNullOrThrow(inventoryEventRegistry);
            this.inventoryPanelPresentation = NullGuard.NotNullOrThrow(inventoryPanelPresentation);
            this.itemDragService = NullGuard.NotNullOrThrow(itemDragService);

            this.combatContextEventRegistry
                .subscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((ICombatContextEventListener)this);
            this.uiCombatContextEventRegistry
                .subscribe(this);

            this.inventoryEventRegistry.subscribe(this);
        }

        public void Dispose() {
            this.combatContextEventRegistry
                .unsubscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((ICombatContextEventListener)this);
            this.uiCombatContextEventRegistry
                .unsubscribe((IUiCombatCharacterSelectedEventListener)this);
            this.inventoryEventRegistry.unsubscribe(this);
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
    }
}