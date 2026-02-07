using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Character.Controller;
using MageFactory.Context;
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

        private void Start() {
            createSlots(new CharacterCreateCommand[] {
                new("Warrior", 120, Team.TeamA),
                new("Mage", 1220, Team.TeamB),
                new("Archer", 1300, Team.TeamB)
            });
        }

        [Inject]
        public void construct(
            ICharacterFactory characterFactory,
            CharacterPrefabAggregate slotPrefab,
            [Inject(Id = "BattleSlotParent")] Transform slotParent,
            CharacterAggregateContext characterAggregateContext
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