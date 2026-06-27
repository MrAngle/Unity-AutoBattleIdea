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
    internal class CombatContextPresentationOrchestrator :
        ICombatCharacterCreatedEventListener,
        ICombatContextEventListener,
        IDisposable,
        IUiCombatCharacterSelectedEventListener,
        IItemPlacedEventEventListener,
        IItemPositionChangedEventListener,
        IHpChangedEventListener,
        ICharacterDeathEventListener,
        IGuardAbsorbedDamageEventListener,
        IFlowGuardCreatedEventListener,
        IFlowStabilityCreatedEventListener,
        IStabilityAbsorbedDamageEventListener,
        IFlowInputStartedEventListener,
        IFlowOutputReachedEventListener,
        IFlowNoOutputEventListener,
        IFlowAttackCreatedEventListener {
        private ICombatContext combatContext;
        private ICombatCharacterFacade selectedCombatCharacter;
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
            this.combatContextEventRegistry
                .subscribe((IFlowGuardCreatedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((IFlowStabilityCreatedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((IFlowInputStartedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((IFlowOutputReachedEventListener)this);
            this.combatContextEventRegistry
                .subscribe((IFlowNoOutputEventListener)this);
            this.combatContextEventRegistry
                .subscribe((IFlowAttackCreatedEventListener)this);
            this.uiCombatContextEventRegistry
                .subscribe(this);
            this.characterEventRegistry
                .subscribe((IHpChangedEventListener)this);
            this.characterEventRegistry
                .subscribe((ICharacterDeathEventListener)this);
            this.characterEventRegistry
                .subscribe((IGuardAbsorbedDamageEventListener)this);
            this.characterEventRegistry
                .subscribe((IStabilityAbsorbedDamageEventListener)this);
            this.inventoryEventRegistry.subscribe((IItemPlacedEventEventListener)this);
            this.inventoryEventRegistry.subscribe((IItemPositionChangedEventListener)this);
        }

        internal static CombatContextPresentationOrchestrator create(
            ICombatContextEventRegistry combatContextEventRegistry,
            IUiCombatContextEventRegistry uiCombatContextEventRegistry,
            IInventoryEventRegistry inventoryEventRegistry,
            ICharacterEventRegistry characterEventRegistry,
            InventoryPanelPresentation inventoryPanelPresentation,
            ItemDragService itemDragService,
            Dictionary<Id<CharacterId>, CharacterPrefabAggregate> characterPrefabs,
            ICombatContext combatContext) {
            return new CombatContextPresentationOrchestrator(
                combatContextEventRegistry,
                uiCombatContextEventRegistry,
                inventoryEventRegistry,
                characterEventRegistry,
                inventoryPanelPresentation,
                itemDragService,
                characterPrefabs,
                combatContext);
        }

        internal void refreshCastProgress() {
            if (selectedCombatCharacter == null) {
                return;
            }

            inventoryPanelPresentation.printItemCastProgress(selectedCombatCharacter.query());
            inventoryPanelPresentation.printDefenseLayers(selectedCombatCharacter.query());
            inventoryPanelPresentation.printPreparedGuards(selectedCombatCharacter.query());
        }

        public void Dispose() {
            this.combatContextEventRegistry
                .unsubscribe((ICombatCharacterCreatedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((ICombatContextEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((IFlowGuardCreatedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((IFlowStabilityCreatedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((IFlowInputStartedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((IFlowOutputReachedEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((IFlowNoOutputEventListener)this);
            this.combatContextEventRegistry
                .unsubscribe((IFlowAttackCreatedEventListener)this);
            this.uiCombatContextEventRegistry
                .unsubscribe((IUiCombatCharacterSelectedEventListener)this);
            this.inventoryEventRegistry.unsubscribe((IItemPlacedEventEventListener)this);
            this.inventoryEventRegistry.unsubscribe((IItemPositionChangedEventListener)this);
            this.characterEventRegistry
                .unsubscribe((IHpChangedEventListener)this);
            this.characterEventRegistry
                .unsubscribe((ICharacterDeathEventListener)this);
            this.characterEventRegistry
                .unsubscribe((IGuardAbsorbedDamageEventListener)this);
            this.characterEventRegistry
                .unsubscribe((IStabilityAbsorbedDamageEventListener)this);
        }

        public void onEvent(in CombatContextCreatedDtoEvent ev) {
            combatContext = ev.combatContext;

            onEvent(new UiCombatCharacterSelectedEvent(combatContext.getRandomCharacter().query().getCharacterInfo()
                .getCharacterId())); // for now
        }

        public void onEvent(in CombatCharacterCreatedDtoEvent ev) {
            // throw new System.NotImplementedException();
            // TODO: update view model
        }

        public void onEvent(in UiCombatCharacterSelectedEvent characterSelectedEvent) {
            selectedCombatCharacter = combatContext.getCombatCharacterById(characterSelectedEvent.characterId);

            inventoryPanelPresentation.printInventory(selectedCombatCharacter.query()
                .getInventoryAggregate());
            refreshCastProgress();

            itemDragService.setCharacterContext(selectedCombatCharacter);
        }

        public void onEvent(in NewItemPlacedDtoEvent ev) {
            ICombatInventoryItemsPanel.NewItemPrintCommand itemPrintCommand =
                new(ev.placedItemId, ev.shapeArchetype, ev.origin, ev.isEntryPoint, ev.entryPointFlowKind,
                    ev.flowPortKind, ev.flowPortName, ev.flowPortDescription);
            inventoryPanelPresentation.printNewItem(itemPrintCommand);
        }

        public void onEvent(in ItemPositionChangedDtoEvent ev) {
            ICombatInventoryItemsPanel.MoveItemToPositionCommand itemPrintCommand =
                new(ev.placedItemId, ev.newOriginPosition);
            inventoryPanelPresentation.moveItemToPosition(itemPrintCommand);
        }

        public void onEvent(in CharacterHpChangedDtoEvent ev) {
            if (characterPrefabs.TryGetValue(ev.characterId, out var prefab)) {
                prefab.hpChange(ev);
            }

            if (isSelectedCharacter(ev.characterId)) {
                inventoryPanelPresentation.printDefenseLayers(selectedCombatCharacter.query());
                inventoryPanelPresentation.showHpChangedVisual(ev.newHp - ev.previousHpValue);
            }
        }

        public void onEvent(in CharacterDeathDtoEvent ev) {
            if (characterPrefabs.TryGetValue(ev.characterId, out var prefab)) {
                prefab.destroy(ev);
            }
        }

        public void onEvent(in CharacterGuardAbsorbedDamageDtoEvent ev) {
            if (characterPrefabs.TryGetValue(ev.characterId, out var prefab)) {
                prefab.guardAbsorbedDamage(ev);
            }

            if (isSelectedCharacter(ev.characterId)) {
                if (ev.hasAffectedGuard) {
                    inventoryPanelPresentation.showGuardAbsorbedVisual(
                        ev.firstAffectedGuardId,
                        ev.blockedDamage);
                }

                inventoryPanelPresentation.printPreparedGuards(selectedCombatCharacter.query());
            }
        }

        public void onEvent(in CharacterStabilityAbsorbedDamageDtoEvent ev) {
            if (characterPrefabs.TryGetValue(ev.characterId, out var prefab)) {
                prefab.stabilityAbsorbedDamage(ev);
            }

            if (isSelectedCharacter(ev.characterId)) {
                inventoryPanelPresentation.printDefenseLayers(selectedCombatCharacter.query());
                inventoryPanelPresentation.showStabilityAbsorbedVisual(
                    ev.reducedDamage,
                    ev.stabilityStrain,
                    ev.remainingDamage);
            }
        }

        public void onEvent(in FlowGuardCreatedDtoEvent ev) {
            if (!isSelectedCharacter(ev.characterId)) {
                return;
            }

            if (ev.replacedGuard) {
                inventoryPanelPresentation.showGuardReplacedVisual(
                    ev.replacedGuardId,
                    ev.replacedGuardPower);
            }

            inventoryPanelPresentation.printPreparedGuards(selectedCombatCharacter.query());

            if (!ev.hasSourceProcessingSlot()) {
                return;
            }

            inventoryPanelPresentation.showGuardCreatedBeam(
                ev.sourceProcessingSlot.getItemId(),
                ev.sourceProcessingSlot.getLocalRow(),
                ev.guardId);
        }

        public void onEvent(in FlowStabilityCreatedDtoEvent ev) {
            if (!isSelectedCharacter(ev.characterId)) {
                return;
            }

            inventoryPanelPresentation.printDefenseLayers(selectedCombatCharacter.query());

            if (!ev.hasSourceProcessingSlot()) {
                return;
            }

            inventoryPanelPresentation.showStabilityGeneratedBeam(
                ev.sourceProcessingSlot.getItemId(),
                ev.sourceProcessingSlot.getLocalRow(),
                ev.stabilityPower);
        }

        public void onEvent(in FlowInputStartedDtoEvent ev) {
            if (!isSelectedCharacter(ev.characterId)) {
                return;
            }

            inventoryPanelPresentation.showFlowInputStarted(ev.inputItemId);
        }

        public void onEvent(in FlowOutputReachedDtoEvent ev) {
            if (!isSelectedCharacter(ev.characterId) || !ev.hasOutputProcessingSlot()) {
                return;
            }

            inventoryPanelPresentation.showFlowOutputReached(
                ev.outputProcessingSlot.getItemId(),
                ev.outputProcessingSlot.getLocalRow(),
                ev.attackPower,
                ev.guardPower,
                ev.stabilityPower);
        }

        public void onEvent(in FlowNoOutputDtoEvent ev) {
            if (!isSelectedCharacter(ev.characterId) || !ev.hasFinalProcessingSlot()) {
                return;
            }

            inventoryPanelPresentation.showFlowNoOutput(
                ev.finalProcessingSlot.getItemId(),
                ev.finalProcessingSlot.getLocalRow(),
                ev.wasCommittedByLegacyRule);
        }

        public void onEvent(in FlowAttackCreatedDtoEvent ev) {
            if (!isSelectedCharacter(ev.sourceCharacterId)
                || !ev.hasSourceProcessingSlot()
                || !characterPrefabs.TryGetValue(ev.targetCharacterId, out CharacterPrefabAggregate targetPrefab)) {
                return;
            }

            inventoryPanelPresentation.showAttackCreatedBeam(
                ev.sourceProcessingSlot.getItemId(),
                ev.sourceProcessingSlot.getLocalRow(),
                targetPrefab.getCenterWorldPosition());
        }

        private bool isSelectedCharacter(Id<CharacterId> characterId) {
            return selectedCombatCharacter != null
                   && selectedCombatCharacter.query().getCharacterInfo().getCharacterId() == characterId;
        }
    }
}