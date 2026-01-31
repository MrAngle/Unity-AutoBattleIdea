using Character.Domain;
using Context;
using Contracts.Character;
using Controller.Character;
using Shared.Utility;
using UnityEngine;
using Zenject;

namespace UI {
    public class BattleUIManager : MonoBehaviour {
        private CharacterPrefabAggregate _slotPrefab;
        private Transform _slotParent;
        private CharacterData[] _team;
        private ICharacterAggregateFactory _characterAggregateFactory;
        private CharacterAggregateContext _characterAggregateContext;

        [Inject]
        public void Construct(
            ICharacterAggregateFactory characterAggregateFactory,
            CharacterPrefabAggregate slotPrefab,
            [Inject(Id = "BattleSlotParent")] Transform slotParent,
            CharacterAggregateContext characterAggregateContext
        ) {
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);
            _characterAggregateFactory = NullGuard.NotNullOrThrow(characterAggregateFactory);
            _slotPrefab = NullGuard.NotNullOrThrow(slotPrefab);
            _slotParent = NullGuard.NotNullOrThrow(slotParent);
            _team = new CharacterData[] {
                new("Warrior", 120),
                new("Mage", 1220),
                new("Archer", 1300)
            };
        }

        private void Start() {
            CreateSlots();
        }

        private void CreateSlots() {
            for (var i = 0; i < _team.Length; i++) {
                ICharacterAggregateFacade characterAggregateFacade;
                if (i == 0) {
                    characterAggregateFacade = _characterAggregateFactory.Create(_team[i], Team.TeamA);
                    _characterAggregateContext.SetCharacterAggregateContext(characterAggregateFacade); // for now
                }
                else {
                    characterAggregateFacade = _characterAggregateFactory.Create(_team[i], Team.TeamB);
                }

                CharacterPrefabAggregate.Create(_slotPrefab, _slotParent,
                    characterAggregateFacade, _characterAggregateContext);
            }
        }
    }
}