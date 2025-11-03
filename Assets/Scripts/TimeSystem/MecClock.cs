using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MEC;
using UnityEngine;

namespace TimeSystem {
   public interface IGameClock
    {
        float TimeScale { get; set; }
        bool IsPaused { get; }
        void Pause();
        void Resume();

        /// Awaitowalne w stylu Task. Kończy się po "seconds" przeskalowanych TimeScale i z uwzględnieniem pauzy.
        Task ContinueIn(float seconds, CancellationToken ct = default);

        /// Harmonogramuje callback po czasie (jak wyżej). Zwraca uchwyt do ewentualnego anulowania.
        CoroutineHandle Schedule(float seconds, Action callback, CancellationToken ct = default);
    }

    /// Implementacja zegara oparta o globalny MEC.Timing z segmentem RealtimeUpdate (niezależny od Time.timeScale).
    internal sealed class MecClock : IGameClock
    {
        private volatile bool _paused;
        private float _timeScale = 1f;

        public string Name { get; }
        public bool IsPaused => _paused;

        public float TimeScale
        {
            get => _timeScale;
            set => _timeScale = Mathf.Max(0f, value);
        }

        public MecClock(string name) => Name = name;

        public void Pause()  => _paused = true;
        public void Resume() => _paused = false;

        public Task ContinueIn(float seconds, CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            CoroutineHandle coroutineHandle = Timing.RunCoroutine(WaitScaled(seconds, 
                () => taskCompletionSource.TrySetResult(null), cancellationToken), Segment.Update);

            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    Timing.KillCoroutines(coroutineHandle);
                    taskCompletionSource.TrySetCanceled();
                });
            }

            return taskCompletionSource.Task;
        }

        public CoroutineHandle Schedule(float seconds, Action callback, CancellationToken ct = default)
        {
            return Timing.RunCoroutine(WaitScaled(seconds, callback, ct), Segment.Update);
        }

        private IEnumerator<float> WaitScaled(float seconds, Action onDone, CancellationToken ct)
        {
            // Mierzymy realny czas (wall clock), więc niezależnie od Unity Time.* i timeScale.
            double remaining = Math.Max(0.0, seconds);
            var sw = Stopwatch.StartNew();
            double last = 0.0;

            while (remaining > 0.0)
            {
                if (ct.IsCancellationRequested)
                    yield break;

                if (!_paused && _timeScale > 0f)
                {
                    double now = sw.Elapsed.TotalSeconds;
                    double dt = now - last;
                    last = now;
                    remaining -= dt * _timeScale;
                }

                // Jedna klatka w Update (MEC Free)
                yield return Timing.WaitForOneFrame;
            }

            onDone?.Invoke();
        }

    }
}