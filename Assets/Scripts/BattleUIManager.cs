using Character;
using UnityEngine;

namespace UI {
    public class BattleUIManager : MonoBehaviour {
        public CharacterPrefabAggregate slotPrefab; // prefab slotu, podłącz w Inspectorze
        public Transform slotParent; // np. Panel / Content w Canvas

        private CharacterData[] team;

        private void Start() {
            // Przykładowe dane drużyny
            team = new CharacterData[] {
                new("Warrior", 120),
                new("Mage", 1220),
                new("Archer", 1300)
            };

            CreateSlots();
        }

        private void CreateSlots() {
            for (var i = 0; i < team.Length; i++)
                CharacterPrefabAggregate.Create(slotPrefab, slotParent, new CharacterAggregate(team[i], Team.TeamA));
        }
    }
}