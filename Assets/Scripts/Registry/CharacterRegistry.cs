using System.Collections.Generic;
using MageFactory.Character.Api;
using UnityEngine;

namespace Registry {
    public class CharacterRegistry : MonoBehaviour {
        private readonly List<ICharacter> _allCharacters = new();
        public static CharacterRegistry Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        public void Register(ICharacter ch) {
            _allCharacters.Add(ch);
        }

        public void Unregister(ICharacter ch) {
            _allCharacters.Remove(ch);
        }

        public List<ICharacter> GetTeamA() {
            // jeśli lista jest pusta — zwróć pustą listę
            if (_allCharacters.Count == 0)
                return new List<ICharacter>();

            // team A — tylko pierwszy element
            var listA = new List<ICharacter>();
            listA.Add(_allCharacters[0]);
            return listA;
        }

        public List<ICharacter> GetTeamB() {
            // jeśli lista ma 0 lub 1 element — nie ma teamu B
            if (_allCharacters.Count <= 1)
                return new List<ICharacter>();

            // team B — pozostałe
            var listB = new List<ICharacter>();
            for (var i = 1; i < _allCharacters.Count; i++) listB.Add(_allCharacters[i]);
            return listB;
        }
    }
}