using Character;
using Context;
using Shared.Utility;
using UnityEngine;
using Zenject;

namespace UI {
    public class BattleUIManager : MonoBehaviour {
        // public CharacterPrefabAggregate slotPrefab; // prefab slotu, podłącz w Inspectorze
        // public Transform slotParent; // np. Panel / Content w Canvas
        //
        // private CharacterData[] team;
        // private ICharacterAggregateFactory _characterAggregateFactory;
        
        private CharacterPrefabAggregate _slotPrefab;
        private Transform _slotParent;
        private CharacterData[] _team;
        private ICharacterAggregateFactory _characterAggregateFactory;
        
        
        [Inject]
        public void Construct(
            ICharacterAggregateFactory characterAggregateFactory,
            CharacterPrefabAggregate slotPrefab,
            [Inject(Id = "BattleSlotParent")] Transform slotParent
            // CharacterData[] team
        ) {
            _characterAggregateFactory = NullGuard.NotNullOrThrow(characterAggregateFactory);
            _slotPrefab = NullGuard.NotNullOrThrow(slotPrefab);
            _slotParent = NullGuard.NotNullOrThrow(slotParent);
            _team = new CharacterData[] {
                new("Warrior", 120),
                new("Mage", 1220),
                new("Archer", 1300)
            };
        }
        

        // [Inject]
        // public BattleUIManager(ICharacterAggregateFactory characterAggregateFactory, CharacterPrefabAggregate slotPrefab, Transform slotParent, CharacterData[] team) {
        //     this.slotPrefab = NullGuard.NotNullOrThrow(slotPrefab);
        //     this.slotParent = NullGuard.NotNullOrThrow(slotParent);
        //     _characterAggregateFactory = NullGuard.NotNullOrThrow(characterAggregateFactory);
        //     this.team = team;
        // }

        private void Start() {
            CreateSlots();
            
            
        }

        private void CreateSlots() {
            for (var i = 0; i < _team.Length; i++) {
                ICharacterAggregateFacade characterAggregateFacade = _characterAggregateFactory.Create(_team[i], Team.TeamA);
                if (i == 0) {
                    CharacterAggregateContext.SetCharacterAggregateContext(characterAggregateFacade); // for now
                }
                CharacterPrefabAggregate.Create(_slotPrefab, _slotParent,
                    characterAggregateFacade);
            }
                

            // new CharacterAggregate(team[i], Team.TeamA));
        }
    }
}