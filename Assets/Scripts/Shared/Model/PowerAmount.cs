using System;

namespace MageFactory.Shared.Model {
    public class PowerAmount {
        protected long power;

        public PowerAmount(long power) {
            this.power = power;
        }

        public long getPower() {
            return power;
        }

        public void change(PowerAmount amount) {
            power += amount.getPower();
        }
    }

    public sealed class DamageToReceive : PowerAmount {
        public static DamageToReceive fromPowerAmount(PowerAmount power) {
            if (power == null) {
                throw new ArgumentNullException(nameof(power)); // for now
            }

            return new DamageToReceive(power.getPower());
        }

        public DamageToReceive(long power) : base(power) {
        }
    }

    public sealed class DamageToDeal : PowerAmount {
        public DamageToDeal(long power) : base(power) {
        }

        public static DamageToDeal fromPowerAmount(PowerAmount power) {
            if (power == null) {
                throw new ArgumentNullException(nameof(power)); // for now
            }

            return new DamageToDeal(power.getPower());
        }

        public static DamageToDeal NO_POWER = new(0);
    }
}