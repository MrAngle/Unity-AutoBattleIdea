using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MageFactory.Character.Api.Event;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.Inventory.Api.Event;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;
using MageFactory.UI.Component;
using MageFactory.UI.Component.Inventory;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Context.Combat.Feature.AddItem;
using UnityEngine;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.UI.Context.Combat {
    public sealed class CombatContextPresentationFactory {
        private readonly ICharacterEventRegistry characterEventRegistry;
        private readonly ICombatContextEventRegistry combatContextEventRegistry;
        private readonly IInventoryEventRegistry inventoryEventRegistry;
        private readonly IUiCombatContextEventRegistry uiCombatContextEventRegistry;

        private readonly IUiCombatContextEventPublisher combatContextEventPublisher;

        private readonly ItemDragService itemDragService;
        private readonly InventoryPanelPresentation inventoryPanelPresentation;

        private readonly CharacterPrefabAggregate characterPrefabAggregate;
        private readonly Transform parentTransformCharacterPrefabAggregate;


        [Inject]
        internal CombatContextPresentationFactory(ICombatContextEventRegistry combatContextEventRegistry,
                                                  IUiCombatContextEventRegistry uiCombatContextEventRegistry,
                                                  IInventoryEventRegistry inventoryEventRegistry,
                                                  ICharacterEventRegistry characterEventRegistry,
                                                  IUiCombatContextEventPublisher combatContextEventPublisher,
                                                  InventoryPanelPresentation inventoryPanelPresentation,
                                                  ItemDragService itemDragService,
                                                  CharacterPrefabAggregate characterPrefabAggregate,
                                                  [Inject(Id = "BattleSlotParent")]
                                                  Transform parentTransformCharacterPrefabAggregate) {
            this.combatContextEventRegistry = NullGuard.NotNullOrThrow(combatContextEventRegistry);
            this.uiCombatContextEventRegistry = NullGuard.NotNullOrThrow(uiCombatContextEventRegistry);
            this.inventoryEventRegistry = NullGuard.NotNullOrThrow(inventoryEventRegistry);
            this.characterEventRegistry = NullGuard.NotNullOrThrow(characterEventRegistry);

            this.combatContextEventPublisher = NullGuard.NotNullOrThrow(combatContextEventPublisher);

            this.inventoryPanelPresentation = NullGuard.NotNullOrThrow(inventoryPanelPresentation);
            this.itemDragService = NullGuard.NotNullOrThrow(itemDragService);

            this.characterPrefabAggregate = NullGuard.NotNullOrThrow(characterPrefabAggregate);
            this.parentTransformCharacterPrefabAggregate =
                NullGuard.NotNullOrThrow(parentTransformCharacterPrefabAggregate);
        }

        public void createCombatContextPresentation(ICombatContext combatContext) {
            Dictionary<Id<CharacterId>, CharacterPrefabAggregate> characterPrefabs = new();
            foreach (var combatCharacter in combatContext.getAllCharacters()) {
                var prefab = CharacterPrefabAggregate.create(
                    characterPrefabAggregate,
                    parentTransformCharacterPrefabAggregate,
                    combatCharacter,
                    combatContextEventPublisher
                );

                characterPrefabs[combatCharacter.getId()] = prefab;
            }

            CombatContextPresentationOrchestrator.create(combatContextEventRegistry,
                uiCombatContextEventRegistry,
                inventoryEventRegistry,
                characterEventRegistry,
                inventoryPanelPresentation,
                itemDragService,
                characterPrefabs,
                combatContext);
        }
    }
}