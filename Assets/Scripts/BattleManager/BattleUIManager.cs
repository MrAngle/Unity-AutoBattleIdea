using System;
using System.Collections;
using MageFactory.Character.Api.Event;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Catalog;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat;
using UnityEngine;
using Zenject;
// using MageFactory.UI.Context.Combat;
// using MageFactory.UI.Context.Combat.Event;

namespace MageFactory.BattleManager {
    public class BattleUIManager : MonoBehaviour {
        // private CharacterPrefabAggregate characterPrefabAggregate;
        private IEntryPointFactory entryPointFactory; // for now
        private Transform slotParent;
        private BattleRuntime battleRuntime; // move to BattleManager
        private Coroutine battleLoop;

        private ICombatContextFactory combatContextFactory;

        // private IUiCombatContextEventPublisher uiCombatContextEventPublisher;
        private ICharacterEventRegistry characterEventRegistry;

        private ICombatContext combatContext;
        private CombatContextPresentationFactory combatContextPresentationFactory;

        private float turnInterval = 2f;

        private void Start() {
            createSlots(new CreateCombatCharacterCommand[] {
                new("Warrior", 120, Team.TeamA, new[] {
                    new EquipItemCommand(
                        EntryPointDefinition.Standard, new Vector2Int(0, 0))
                }),
                new("Mage", 1220, Team.TeamB, Array.Empty<EquipItemCommand>()),
                new("Archer", 1300, Team.TeamB, Array.Empty<EquipItemCommand>())
            });

            startBattleLoop();
        }

        [Inject]
        public void construct([Inject(Id = "BattleSlotParent")] Transform injectSlotParent,
                              BattleRuntime injectBattleRuntime,
                              ICombatContextFactory injectCombatContextFactory,
                              ICharacterEventRegistry injectCharacterEventRegistry,
                              CombatContextPresentationFactory injectCombatContextPresentationFactory) {
            slotParent = NullGuard.NotNullOrThrow(injectSlotParent);
            battleRuntime = NullGuard.NotNullOrThrow(injectBattleRuntime);
            combatContextFactory = NullGuard.NotNullOrThrow(injectCombatContextFactory);
            characterEventRegistry = NullGuard.NotNullOrThrow(injectCharacterEventRegistry);
            combatContextPresentationFactory = NullGuard.NotNullOrThrow(injectCombatContextPresentationFactory);
        }

        private void createSlots(CreateCombatCharacterCommand[] charactersToCreate) {
            combatContext = combatContextFactory.create(charactersToCreate);

            combatContextPresentationFactory.createCombatContextPresentation(combatContext);

            // foreach (ICombatCharacter combatCharacter in combatContext.getAllCharacters()) {
            //     CharacterPrefabAggregate.create(characterPrefabAggregate, slotParent, combatCharacter,
            //         uiCombatContextEventPublisher, characterEventRegistry);
            // }
        }

        private void startBattleLoop() {
            battleLoop = StartCoroutine(executeLoop());
        }

        private IEnumerator executeLoop() {
            var wait = new WaitForSeconds(turnInterval);

            while (true) {
                battleRuntime.tick(combatContext);
                yield return wait;
            }
        }
    }
}