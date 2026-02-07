using System.Collections.Generic;
using MageFactory.Character.Api;
using UnityEngine;

namespace MageFactory.Registry {
    public class CharacterRegistry : MonoBehaviour {
        private readonly List<ICharacter> allCharacters = new();
        public static CharacterRegistry Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        public void register(ICharacter ch) {
            allCharacters.Add(ch);
        }

        public void unregister(ICharacter ch) {
            allCharacters.Remove(ch);
        }

        public List<ICharacter> getTeamA() {
            // jeśli lista jest pusta — zwróć pustą listę
            if (allCharacters.Count == 0)
                return new List<ICharacter>();

            // team A — tylko pierwszy element
            var listA = new List<ICharacter>();
            listA.Add(allCharacters[0]);
            return listA;
        }

        public List<ICharacter> getTeamB() {
            // jeśli lista ma 0 lub 1 element — nie ma teamu B
            if (allCharacters.Count <= 1)
                return new List<ICharacter>();

            // team B — pozostałe
            var listB = new List<ICharacter>();
            for (var i = 1; i < allCharacters.Count; i++) listB.Add(allCharacters[i]);
            return listB;
        }
    }
}