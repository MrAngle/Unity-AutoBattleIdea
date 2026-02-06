using UnityEngine;

namespace BattleManager {
    public class BattleManager : MonoBehaviour {
        [SerializeField] private float turnInterval = 1.5f; // co ile sekund tura

        private bool _battleRunning;

        private void Start() {
            // Na razie testowo – można w przyszłości inicjalizować z prefabu
            StartBattle();
        }

        public void StartBattle() {
            if (_battleRunning) return;
            _battleRunning = true;
            // StartCoroutine(BattleLoop());
        }
    }
}