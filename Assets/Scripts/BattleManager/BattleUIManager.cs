using Context;
using MageFactory.Character.Api;
using MageFactory.Character.Api.Dto;
using MageFactory.Character.Controller;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace UI {
    public class BattleUIManager : MonoBehaviour {
        private CharacterAggregateContext _characterAggregateContext;
        private ICharacterAggregateFactory _characterAggregateFactory;
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
            ICharacterAggregateFactory characterAggregateFactory,
            CharacterPrefabAggregate slotPrefab,
            [Inject(Id = "BattleSlotParent")] Transform slotParent,
            CharacterAggregateContext characterAggregateContext
        ) {
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);
            _characterAggregateFactory = NullGuard.NotNullOrThrow(characterAggregateFactory);
            _slotPrefab = NullGuard.NotNullOrThrow(slotPrefab);
            _slotParent = NullGuard.NotNullOrThrow(slotParent);
        }

        private void createSlots(CharacterCreateCommand[] charactersToCreate) {
            for (var i = 0; i < charactersToCreate.Length; i++) {
                ICharacter character;
                // TODO: change it of course
                if (i == 0) {
                    character = _characterAggregateFactory.create(charactersToCreate[i]);
                    _characterAggregateContext.SetCharacterAggregateContext(character); // for now
                }
                else {
                    character = _characterAggregateFactory.create(charactersToCreate[i]);
                }

                CharacterPrefabAggregate.Create(_slotPrefab, _slotParent, character, _characterAggregateContext);
            }
        }
    }
}