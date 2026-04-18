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

        public static DamageToReceive fromDamageToDeal(DamageToDeal damageToDeal) {
            if (damageToDeal == null) {
                throw new ArgumentNullException(nameof(damageToDeal));
            }

            return new DamageToReceive(damageToDeal.getPower());
        }

        public DamageToReceive(long power) : base(power) {
        }
    }

    public sealed class ResolvedDamage : PowerAmount {
        public static readonly ResolvedDamage NO_DAMAGE = new(0);

        public ResolvedDamage(long power) : base(power) {
        }

        public static ResolvedDamage fromPowerAmount(PowerAmount power) {
            if (power == null) {
                throw new ArgumentNullException(nameof(power));
            }

            return new ResolvedDamage(power.getPower());
        }

        public static ResolvedDamage fromDamageToReceive(DamageToReceive damageToReceive) {
            if (damageToReceive == null) {
                throw new ArgumentNullException(nameof(damageToReceive));
            }

            return new ResolvedDamage(damageToReceive.getPower());
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


    public sealed class DamageTaken : PowerAmount {
        public DamageTaken(long power) : base(power) {
        }

        public static DamageTaken fromDamageToReceive(DamageToReceive damageToReceive) {
            if (damageToReceive == null) {
                throw new ArgumentNullException(nameof(damageToReceive));
            }

            return new DamageTaken(damageToReceive.getPower());
        }
    }
}