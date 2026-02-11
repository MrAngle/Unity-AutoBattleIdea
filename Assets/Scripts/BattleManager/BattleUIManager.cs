using System;
using MageFactory.Character.Controller;
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

        private void Start() {
            createSlots(new CreateCombatCharacterCommand[] {
                new("Warrior", 120, Team.TeamA, new[] {
                    new EquipItemCommand(
                        EntryPointDefinition.Standard, new Vector2Int(0, 0))
                }),
                new("Mage", 1220, Team.TeamB, Array.Empty<EquipItemCommand>()),
                new("Archer", 1300, Team.TeamB, Array.Empty<EquipItemCommand>())
            });
        }

        [Inject]
        public void construct(
            ICharacterFactory injectCharacterFactory,
            CharacterPrefabAggregate injectSlotPrefab,
            [Inject(Id = "BattleSlotParent")] Transform injectSlotParent,
            CharacterAggregateContext injectCharacterAggregateContext) {
            characterAggregateContext = NullGuard.NotNullOrThrow(injectCharacterAggregateContext);
            characterFactory = NullGuard.NotNullOrThrow(injectCharacterFactory);
            characterPrefabAggregate = NullGuard.NotNullOrThrow(injectSlotPrefab);
            slotParent = NullGuard.NotNullOrThrow(injectSlotParent);
        }

        private void createSlots(CreateCombatCharacterCommand[] charactersToCreate) {
            for (var i = 0; i < charactersToCreate.Length; i++) {
                ICombatCharacter character;
                // TODO: change it of course
                if (i == 0) {
                    character = characterFactory.create(charactersToCreate[i]);
                    characterAggregateContext.setCharacterAggregateContext(character); // for now
                }
                else {
                    character = characterFactory.create(charactersToCreate[i]);
                }

                CharacterPrefabAggregate.create(characterPrefabAggregate, slotParent, character,
                    characterAggregateContext);
            }
        }
    }
}