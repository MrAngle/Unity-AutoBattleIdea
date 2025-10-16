using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Registry {
    public class CharacterRegistry : MonoBehaviour {
        private readonly List<CharacterAggregate> _allCharacters = new();
        public static CharacterRegistry Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        public void Register(CharacterAggregate ch) {
            _allCharacters.Add(ch);
        }

        public void Unregister(CharacterAggregate ch) {
            _allCharacters.Remove(ch);
        }

        public List<CharacterAggregate> GetTeamA() {
            // jeśli lista jest pusta — zwróć pustą listę
            if (_allCharacters.Count == 0)
                return new List<CharacterAggregate>();

            // team A — tylko pierwszy element
            var listA = new List<CharacterAggregate>();
            listA.Add(_allCharacters[0]);
            return listA;
        }

        public List<CharacterAggregate> GetTeamB() {
            // jeśli lista ma 0 lub 1 element — nie ma teamu B
            if (_allCharacters.Count <= 1)
                return new List<CharacterAggregate>();

            // team B — pozostałe
            var listB = new List<CharacterAggregate>();
            for (var i = 1; i < _allCharacters.Count; i++) listB.Add(_allCharacters[i]);
            return listB;
        }
    }
}