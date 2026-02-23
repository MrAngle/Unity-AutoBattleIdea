using System;
using System.Runtime.CompilerServices;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.Inventory.Controller;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat.Event;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.UI.Context.Combat {
    internal class CombatContextPresentationHandler : ICombatCharacterCreatedEventListener, ICombatContextEventListener,
        IDisposable, IUiCombatCharacterSelectedEventListener {
        private ICombatContext combatContext; // TODO: change to "view model"
        private ICombatCharacter selectedCharacter;
        private readonly ICombatContextEventRegistry combatContextEventRegistry;
        private readonly IUiCombatContextEventRegistry uiCombatContextEventRegistry;
        private readonly ICombatInventoryGridPanel combatInventoryGridPanel;
        private readonly ICombatInventoryItemsPanel combatInventoryItemsPanel;
        private readonly ItemDragService itemDragService;

        [Inject]
        public CombatContextPresentationHandler(ICombatContextEventRegistry combatContextEventRegistry,
                                                ICombatInventoryGridPanel combatInventoryGridPanel,
                                                IUiCombatContextEventRegistry uiCombatContextEventRegistry,
                                                ICombatInventoryItemsPanel combatInventoryItemsPanel,
                                                ItemDragService itemDragService) {
            this.combatContextEventRegistry = NullGuard.NotNullOrThrow(combatContextEventRegistry);
            this.uiCombatContextEventRegistry = NullGuard.NotNullOrThrow(uiCombatContextEventRegistry);
            this.combatInventoryGridPanel = NullGuard.NotNullOrThrow(combatInventoryGridPanel);
            this.combatInventoryItemsPanel = NullGuard.NotNullOrThrow(combatInventoryItemsPanel);
            this.itemDragService = NullGuard.NotNullOrThrow(itemDragService);

            this.combatContextEventRegistry
                .subscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((ICombatContextEventListener)this);
            this.uiCombatContextEventRegistry
                .subscribe(this);
        }

        public void onEvent(in CombatCharacterCreatedDtoEvent ev) {
            // throw new System.NotImplementedException();
            // TODO: update view model
        }

        public void onEvent(in CombatContextCreatedDtoEvent ev) {
            combatContext = ev.combatContext;

            onEvent(new UiCombatCharacterSelectedEvent(combatContext.getRandomCharacter().getId())); // for now
        }

        public void onEvent(in UiCombatCharacterSelectedEvent characterSelectedEvent) {
            selectedCharacter = combatContext.getCombatCharacterById(characterSelectedEvent.characterId);

            ICombatCharacterInventory combatCharacterInventory = selectedCharacter.getInventoryAggregate();
            ICombatInventory combatInventory = combatCharacterInventory.getInventoryGrid();
            ICombatInventory invReferenceCopy = combatInventory;

            ICombatInventoryGridPanel.UiPrintInventoryGridCommand printInventoryGridCommand =
                new ICombatInventoryGridPanel.UiPrintInventoryGridCommand(
                    invReferenceCopy.Width,
                    invReferenceCopy.Height,
                    coord => invReferenceCopy.getState(coord));
            combatInventoryGridPanel.printInventoryGrid(printInventoryGridCommand);

            ICombatInventoryItemsPanel.UiPrintInventoryItemsCommand itemsCommand =
                new(combatCharacterInventory.getPlacedSnapshot());
            combatInventoryItemsPanel.printInventoryItems(itemsCommand);

            this.itemDragService.setCharacterContext(selectedCharacter);
        }

        public void Dispose() {
            this.combatContextEventRegistry
                .unsubscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((ICombatContextEventListener)this);
            this.uiCombatContextEventRegistry
                .unsubscribe((IUiCombatCharacterSelectedEventListener)this);
        }
    }
}