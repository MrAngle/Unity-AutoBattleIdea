using UnityEngine;

namespace MageFactory.BattleManager {
    public class BattleManager : MonoBehaviour {
        [SerializeField] private float turnInterval = 1.5f; // co ile sekund tura

        private bool _battleRunning;

        private void Start() {
            startBattle();
        }

        private void startBattle() {
            if (_battleRunning) return;
            _battleRunning = true;
        }
    }
}