using System.Collections;
using System.Collections.Generic;
using Character;
using Registry;
using UnityEngine;

namespace BattleManager {
    public class BattleManager : MonoBehaviour {
        [SerializeField] private float turnInterval = 1.5f; // co ile sekund tura

        private bool _battleRunning;
        private List<CharacterAggregate> _teamA;
        private List<CharacterAggregate> _teamB;

        private void Start() {
            _teamA = CharacterRegistry.Instance.GetTeamA();
            _teamB = CharacterRegistry.Instance.GetTeamB();
            // Na razie testowo – można w przyszłości inicjalizować z prefabu
            StartBattle();
        }

        public void StartBattle() {
            if (_battleRunning) return;
            _battleRunning = true;
            StartCoroutine(BattleLoop());
        }

        private IEnumerator BattleLoop() {
            while (_battleRunning) {
                yield return new WaitForSeconds(turnInterval);

                // wybierz losowego atakującego z drużyny A
                var attacker = CharacterRegistry.Instance.GetTeamA()[Random.Range(0, _teamA.Count)];
                var target = CharacterRegistry.Instance.GetTeamB()[Random.Range(0, _teamB.Count)];

                var dmg = Random.Range(5, 15);
                target.TakeDamage(dmg);

                Debug.Log($"{attacker.Name} zadał {dmg} obrażeń {target.Name}");

                // w przyszłości: update UI eventem
            }
        }
    }
}