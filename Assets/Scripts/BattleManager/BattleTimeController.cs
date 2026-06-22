using System;
using MageFactory.Shared.Utility;

namespace MageFactory.BattleManager {
    /// <summary>
    /// Converts real elapsed time into deterministic combat ticks without changing combat-domain tick semantics.
    /// </summary>
    public sealed class BattleTimeController {
        public const float MinSpeedMultiplier = 0.2f;
        public const float MaxSpeedMultiplier = 5f;
        public const float DefaultSpeedMultiplier = 1f;

        private const double ReadyTickEpsilon = 0.000001d;

        private readonly BattleSessionSettings settings;
        private double accumulatedCombatTicks;
        private float speedMultiplier = DefaultSpeedMultiplier;
        private bool paused;

        public BattleTimeController(BattleSessionSettings settings) {
            this.settings = NullGuard.NotNullOrThrow(settings);
        }

        public float getSpeedMultiplier() {
            return speedMultiplier;
        }

        public bool isPaused() {
            return paused;
        }

        public void setSpeedMultiplier(float speedMultiplier) {
            if (float.IsNaN(speedMultiplier)
                || float.IsInfinity(speedMultiplier)
                || speedMultiplier < MinSpeedMultiplier
                || speedMultiplier > MaxSpeedMultiplier) {
                throw new ArgumentOutOfRangeException(
                    nameof(speedMultiplier),
                    speedMultiplier,
                    $"Battle speed must be between {MinSpeedMultiplier}x and {MaxSpeedMultiplier}x.");
            }

            this.speedMultiplier = speedMultiplier;
        }

        public void pause() {
            paused = true;
        }

        public void resume() {
            paused = false;
        }

        public void togglePause() {
            paused = !paused;
        }

        public int consumeReadyCombatTicks(float unscaledDeltaSeconds) {
            if (float.IsNaN(unscaledDeltaSeconds)
                || float.IsInfinity(unscaledDeltaSeconds)
                || unscaledDeltaSeconds < 0f) {
                throw new ArgumentOutOfRangeException(
                    nameof(unscaledDeltaSeconds),
                    unscaledDeltaSeconds,
                    "Elapsed time must be a finite non-negative value.");
            }

            if (paused || unscaledDeltaSeconds == 0f) {
                return 0;
            }

            accumulatedCombatTicks += unscaledDeltaSeconds
                                      * speedMultiplier
                                      * settings.getCombatTicksPerRealSecond();

            int readyTicks = (int)Math.Floor(accumulatedCombatTicks + ReadyTickEpsilon);

            if (readyTicks <= 0) {
                return 0;
            }

            accumulatedCombatTicks -= readyTicks;

            if (accumulatedCombatTicks < 0d && accumulatedCombatTicks > -ReadyTickEpsilon) {
                accumulatedCombatTicks = 0d;
            }

            return readyTicks;
        }
    }
}