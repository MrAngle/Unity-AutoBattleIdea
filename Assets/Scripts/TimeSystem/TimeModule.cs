using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MEC;

namespace TimeSystem {
    /// Główny "hub" – zarządza kolekcją zegarów i udostępnia prosty statyczny API.
    public static class TimeModule {
        // Predefiniowane nazwy (opcjonalne). Możesz tworzyć własne przez Get("twoja_nazwa").
        public const string DEFAULT = "default";
        public const string CRAFTING = "crafting";
        public const string COMBAT = "combat";
        public const string UI = "ui";

        private static readonly Dictionary<string, IGameClock> _clocks = new(StringComparer.OrdinalIgnoreCase);

        static TimeModule() {
            // Wstępnie kilka zegarów – możesz to zmienić według potrzeb.
            Ensure(DEFAULT);
            Ensure(CRAFTING);
            Ensure(COMBAT);
            Ensure(UI);
        }

        /// Pobiera istniejący lub tworzy nowy zegar o danej nazwie.
        public static IGameClock Get(string name) => Ensure(name);

        /// Uproszczone API działające na zegarze "default" – jak w Twoim przykładzie.
        public static Task ContinueIn(float seconds, CancellationToken ct = default) =>
            Get(DEFAULT).ContinueIn(seconds, ct);

        public static CoroutineHandle Schedule(float seconds, Action callback, CancellationToken ct = default) =>
            Get(DEFAULT).Schedule(seconds, callback, ct);

        public static void Pause() => Get(DEFAULT).Pause();
        public static void Resume() => Get(DEFAULT).Resume();

        public static float TimeScale {
            get => Get(DEFAULT).TimeScale;
            set => Get(DEFAULT).TimeScale = value;
        }

        private static IGameClock Ensure(string name) {
            if (!_clocks.TryGetValue(name, out var clock)) {
                clock = new MecClock(name);
                _clocks[name] = clock;
            }

            return clock;
        }
    }
}