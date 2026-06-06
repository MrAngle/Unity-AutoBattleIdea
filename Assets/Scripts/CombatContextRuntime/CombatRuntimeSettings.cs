using System;

namespace MageFactory.CombatContextRuntime {
    public enum CombatRuntimeProfile {
        Developer,
        Production
    }

    public sealed class CombatRuntimeSettings {
        private readonly CombatRuntimeProfile profile;

        public CombatRuntimeSettings(CombatRuntimeProfile profile) {
            if (!Enum.IsDefined(typeof(CombatRuntimeProfile), profile)) {
                throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unsupported combat runtime profile.");
            }

            this.profile = profile;
        }

        public static CombatRuntimeSettings developer() {
            return new CombatRuntimeSettings(CombatRuntimeProfile.Developer);
        }

        public static CombatRuntimeSettings production() {
            return new CombatRuntimeSettings(CombatRuntimeProfile.Production);
        }

        public CombatRuntimeProfile getProfile() {
            return profile;
        }

        public bool shouldLogCombatHotPath() {
            return profile == CombatRuntimeProfile.Developer;
        }
    }
}