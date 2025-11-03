using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace TimeSystem {

    [DisallowMultipleComponent]
    public class ProcessTimer : MonoBehaviour {
        [Header("Clock")] [Tooltip("Nazwa zegara z TimeModule (np. 'crafting', 'combat').")] [SerializeField]
        private string clockName = TimeModule.CRAFTING;

        [Header("Duration (units of the selected clock)")] [Min(0f)] [SerializeField]
        private float duration = 5f;

        [Header("Events")] public UnityEvent onStarted;

        public UnityEvent onCompleted;
        public UnityEvent onCanceled;

        private CancellationTokenSource _cts;

        public bool IsRunning => _cts != null;

        private void OnDisable() {
            Cancel();
        }

        /// Rozpoczyna proces. Jeśli już trwa – restartuje.
        public async void StartProcessing() {
            Cancel(); // bezpieczny restart
            _cts = new CancellationTokenSource();

            try {
                onStarted?.Invoke();
                var clock = TimeModule.Get(clockName);
                await clock.ContinueIn(duration, _cts.Token);
                onCompleted?.Invoke();
            }
            catch (TaskCanceledException) {
                onCanceled?.Invoke();
            }
            finally {
                _cts?.Dispose();
                _cts = null;
            }
        }

        /// Anuluje bieżący proces (jeśli trwa).
        public void Cancel() {
            if (_cts == null) return;
            if (!_cts.IsCancellationRequested)
                _cts.Cancel();
        }
    }
}