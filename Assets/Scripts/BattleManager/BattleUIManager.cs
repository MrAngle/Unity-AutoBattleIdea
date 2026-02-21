using System;
using System.Collections;
using MageFactory.Character.Controller;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Context;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Catalog;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.BattleManager {
    public class BattleUIManager : MonoBehaviour {
        private CharacterAggregateContext characterAggregateContext;
        private ICharacterFactory characterFactory;
        private CharacterPrefabAggregate characterPrefabAggregate;
        private IEntryPointFactory entryPointFactory; // for now
        private Transform slotParent;
        private BattleRuntime battleRuntime; // move to BattleManager
        private Coroutine battleLoop;
        private ICombatContextFactory combatContextFactory;

        private ICombatContext combatContext;

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
        public void construct(ICharacterFactory injectCharacterFactory,
                              CharacterPrefabAggregate injectSlotPrefab,
                              [Inject(Id = "BattleSlotParent")] Transform injectSlotParent,
                              CharacterAggregateContext injectCharacterAggregateContext,
                              BattleRuntime injectBattleRuntime,
                              ICombatContextFactory injectCombatContextFactory) {
            characterAggregateContext = NullGuard.NotNullOrThrow(injectCharacterAggregateContext);
            characterFactory = NullGuard.NotNullOrThrow(injectCharacterFactory);
            characterPrefabAggregate = NullGuard.NotNullOrThrow(injectSlotPrefab);
            slotParent = NullGuard.NotNullOrThrow(injectSlotParent);
            battleRuntime = NullGuard.NotNullOrThrow(injectBattleRuntime);
            combatContextFactory = NullGuard.NotNullOrThrow(injectCombatContextFactory);
        }

        private void createSlots(CreateCombatCharacterCommand[] charactersToCreate) {
            combatContext = combatContextFactory.create(charactersToCreate);

            characterAggregateContext.setCharacterAggregateContext(combatContext.getRandomCharacter()); // for now

            foreach (ICombatCharacter combatCharacter in combatContext.getAllCharacters()) {
                battleRuntime.register(combatCharacter);
                CharacterPrefabAggregate.create(characterPrefabAggregate, slotParent, combatCharacter,
                    characterAggregateContext);
            }
        }

        private void startBattleLoop() {
            battleLoop = StartCoroutine(executeLoop());
        }

        private IEnumerator executeLoop() {
            var wait = new WaitForSeconds(turnInterval);

            while (true) {
                battleRuntime.tick();
                yield return wait;
            }
        }
    }
}