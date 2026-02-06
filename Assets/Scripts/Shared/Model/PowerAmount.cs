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
        public DamageToReceive(long power) : base(power) {
        }
    }

    public sealed class DamageToDeal : PowerAmount {
        public DamageToDeal(long power) : base(power) {
        }
    }
}