using Character;
using UnityEngine;

namespace UI
{
    public class BattleUIManager : MonoBehaviour
    {
        public CharacterSlotUI slotPrefab;   // prefab slotu, podłącz w Inspectorze
        public Transform slotParent;         // np. Panel / Content w Canvas

        private CharacterData[] team;

        void Start()
        {
            // Przykładowe dane drużyny
            team = new CharacterData[]
            {
                new CharacterData("Warrior", 120),
                new CharacterData("Mage", 80),
                new CharacterData("Archer", 100)
            };

            CreateSlots();
        }

        void CreateSlots()
        {
            for (int i = 0; i < team.Length; i++)
            {
                CharacterSlotUI slot = Instantiate(slotPrefab, slotParent, false);
                slot.SetCharacter(team[i]);
            }
        }

        // metoda testowa: zadawanie obrażeń np. dla slotu o danym indeksie
        public void DealDamageTo(int index, int damage)
        {
            if (index < 0 || index >= team.Length) return;

            team[index].TakeDamage(damage);

            // jeśli chcesz, możesz też zapisać referencje do slotów
            // i wywołać slot.RefreshUI() dla tego, który się zmienił
        }
    }
}