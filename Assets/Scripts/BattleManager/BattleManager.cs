using UnityEngine;
using Zenject;

namespace MageFactory.BattleManager {
    public class BattleManager : MonoBehaviour {
        [SerializeField] private float turnInterval = 1.5f;

        private BattleRuntime _runtime;
        private Coroutine loop;

        [Inject]
        public void construct(BattleRuntime runtime) {
            _runtime = runtime;
        }

        // private void Start() {
        //     loop = StartCoroutine(executeLoop());
        // }
        //
        // private IEnumerator executeLoop() {
        //     var wait = new WaitForSeconds(turnInterval);
        //
        //     while (true) {
        //         _runtime.tick();
        //         yield return wait;
        //     }
        // }
    }
}