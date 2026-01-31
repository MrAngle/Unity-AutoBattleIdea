using System.Collections.Generic;
using Contracts.Character;
using UnityEngine;

namespace Registry {
    public class CharacterRegistry : MonoBehaviour {
        private readonly List<ICharacterAggregateFacade> _allCharacters = new();
        public static CharacterRegistry Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        public void Register(ICharacterAggregateFacade ch) {
            _allCharacters.Add(ch);
        }

        public void Unregister(ICharacterAggregateFacade ch) {
            _allCharacters.Remove(ch);
        }

        public List<ICharacterAggregateFacade> GetTeamA() {
            // jeśli lista jest pusta — zwróć pustą listę
            if (_allCharacters.Count == 0)
                return new List<ICharacterAggregateFacade>();

            // team A — tylko pierwszy element
            var listA = new List<ICharacterAggregateFacade>();
            listA.Add(_allCharacters[0]);
            return listA;
        }

        public List<ICharacterAggregateFacade> GetTeamB() {
            // jeśli lista ma 0 lub 1 element — nie ma teamu B
            if (_allCharacters.Count <= 1)
                return new List<ICharacterAggregateFacade>();

            // team B — pozostałe
            var listB = new List<ICharacterAggregateFacade>();
            for (var i = 1; i < _allCharacters.Count; i++) listB.Add(_allCharacters[i]);
            return listB;
        }
    }
}