using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain {
    internal class FlowPendingDamage {
        private readonly DamageRole damageRole;
        private PowerAmount power;

        internal FlowPendingDamage(DamageRole damageRole, PowerAmount initialPower) {
            this.damageRole = NullGuard.enumDefinedOrThrow(damageRole);
            this.power = NullGuard.NotNullOrThrow(initialPower.getPower() > 0 ? initialPower : PowerAmount.noPower());
        }

        internal PowerAmount getPower() {
            return power;
        }

        internal void changePower(PowerAmount powerDelta) {
            power.change(powerDelta);

            if (power.getPower() < 0) {
                power = PowerAmount.noPower();
            }
        }

        internal bool hasNoEffect() {
            return power.getPower() <= 0;
        }
    }
}