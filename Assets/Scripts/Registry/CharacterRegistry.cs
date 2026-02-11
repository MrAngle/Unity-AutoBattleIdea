using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using UnityEngine;

namespace MageFactory.Registry {
    public class CharacterRegistry : MonoBehaviour {
        private readonly List<ICombatCharacter> allCharacters = new();
        public static CharacterRegistry Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        public void register(ICombatCharacter ch) {
            allCharacters.Add(ch);
        }

        public void unregister(ICombatCharacter ch) {
            allCharacters.Remove(ch);
        }

        public List<ICombatCharacter> getTeamA() {
            // jeśli lista jest pusta — zwróć pustą listę
            if (allCharacters.Count == 0)
                return new List<ICombatCharacter>();

            // team A — tylko pierwszy element
            var listA = new List<ICombatCharacter>();
            listA.Add(allCharacters[0]);
            return listA;
        }

        public List<ICombatCharacter> getTeamB() {
            // jeśli lista ma 0 lub 1 element — nie ma teamu B
            if (allCharacters.Count <= 1)
                return new List<ICombatCharacter>();

            // team B — pozostałe
            var listB = new List<ICombatCharacter>();
            for (var i = 1; i < allCharacters.Count; i++) listB.Add(allCharacters[i]);
            return listB;
        }
    }
}