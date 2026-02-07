using System;
using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Character.Controller;
using MageFactory.Context;
using MageFactory.Inventory.Contract;
using MageFactory.Item.Catalog;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.BattleManager {
    public class BattleUIManager : MonoBehaviour {
        private CharacterAggregateContext _characterAggregateContext;
        private ICharacterFactory characterFactory;
        private Transform _slotParent;
        private CharacterPrefabAggregate _slotPrefab;
        private IEntryPointFactory _entryPointFactory; // for now

        private void Start() {
            createSlots(new CharacterCreateCommand[] {
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
            ICharacterFactory characterFactory,
            CharacterPrefabAggregate slotPrefab,
            [Inject(Id = "BattleSlotParent")] Transform slotParent,
            CharacterAggregateContext characterAggregateContext,
            IEntryPointFactory _entryPointFactory
        ) {
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            _slotPrefab = NullGuard.NotNullOrThrow(slotPrefab);
            _slotParent = NullGuard.NotNullOrThrow(slotParent);
        }

        private void createSlots(CharacterCreateCommand[] charactersToCreate) {
            for (var i = 0; i < charactersToCreate.Length; i++) {
                ICharacter character;
                // TODO: change it of course
                if (i == 0) {
                    character = characterFactory.create(charactersToCreate[i]);
                    _characterAggregateContext.setCharacterAggregateContext(character); // for now
                }
                else {
                    character = characterFactory.create(charactersToCreate[i]);
                }

                CharacterPrefabAggregate.create(_slotPrefab, _slotParent, character, _characterAggregateContext);
            }
        }
    }
}