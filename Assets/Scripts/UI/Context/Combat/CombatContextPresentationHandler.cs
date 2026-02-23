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
        private readonly ICombatContextEventRegistry combatContextEventRegistry;
        private readonly IUiCombatContextEventRegistry uiCombatContextEventRegistry;
        private readonly ICombatInventoryPanel combatInventoryPanel;

        [Inject]
        public CombatContextPresentationHandler(ICombatContextEventRegistry combatContextEventRegistry,
                                                ICombatInventoryPanel combatInventoryPanel,
                                                IUiCombatContextEventRegistry uiCombatContextEventRegistry) {
            this.combatContextEventRegistry = NullGuard.NotNullOrThrow(combatContextEventRegistry);
            this.uiCombatContextEventRegistry = NullGuard.NotNullOrThrow(uiCombatContextEventRegistry);
            this.combatInventoryPanel = NullGuard.NotNullOrThrow(combatInventoryPanel);

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
            onEvent(new UiCombatCharacterSelectedEvent(combatContext.getRandomCharacter().getId()));
        }

        public void onEvent(in UiCombatCharacterSelectedEvent characterSelectedEvent) {
            ICombatCharacter combatCharacter =
                combatContext.getCombatCharacterById(characterSelectedEvent.characterId);

            ICombatCharacterInventory combatCharacterInventory = combatCharacter.getInventoryAggregate();
            ICombatInventory combatInventory = combatCharacterInventory.getInventoryGrid();
            ICombatInventory invReferenceCopy = combatInventory;

            ICombatInventoryPanel.UiChangeInventoryCommand changeInventoryCommand =
                new ICombatInventoryPanel.UiChangeInventoryCommand(
                    invReferenceCopy.Width,
                    invReferenceCopy.Height,
                    coord => invReferenceCopy.getState(coord));

            combatInventoryPanel.changeInventory(changeInventoryCommand);
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