using UnityEngine;
using Zenject;

namespace MageFactory.BattleManager {
    public class BattleManager : MonoBehaviour {
        private BattleRuntime _runtime;
        private Coroutine loop;

        [Inject]
        public void construct(BattleRuntime runtime) {
            _runtime = runtime;
        }
    }
}