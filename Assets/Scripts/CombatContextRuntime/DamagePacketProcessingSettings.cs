using System;
using MageFactory.Shared.Model;

namespace MageFactory.CombatContextRuntime {
    public sealed class DamagePacketProcessingSettings {
        private static readonly CombatTicks DefaultTicksPerStage = CombatTicks.of(10);

        private readonly CombatTicks ticksPerStage;

        public DamagePacketProcessingSettings(CombatTicks ticksPerStage) {
            if (!ticksPerStage.isPositive()) {
                throw new ArgumentOutOfRangeException(
                    nameof(ticksPerStage),
                    ticksPerStage,
                    "Damage packet stage duration must be positive.");
            }

            this.ticksPerStage = ticksPerStage;
        }

        public static DamagePacketProcessingSettings defaults() {
            return new DamagePacketProcessingSettings(DefaultTicksPerStage);
        }

        public CombatTicks getTicksPerStage() {
            return ticksPerStage;
        }
    }
}