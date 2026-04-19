using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Domain {
    internal class FlowEffectBucket {
        private readonly FlowPendingDamage pendingDamage;

        internal FlowEffectBucket(DamageRole damageRole, PowerAmount initialPower) {
            pendingDamage = new FlowPendingDamage(
                damageRole,
                initialPower
            );
        }

        internal PowerAmount getPower() {
            return pendingDamage.getPower();
        }

        internal void changePower(PowerAmount powerDelta) {
            pendingDamage.changePower(powerDelta);
        }

        internal bool hasNoEffect() {
            return pendingDamage.hasNoEffect();
        }
    }
}