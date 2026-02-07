using System;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Domain {
    public class FlowPayload {
        private readonly DamageToDeal _damageToDeal;
        private readonly DamageToReceive _damageToReceive;

        internal FlowPayload(long power, DamageToReceive damageToReceiveToReceive = null,
            DamageToDeal damageToDeal = null) {
            _damageToReceive = damageToReceiveToReceive ?? new DamageToReceive(0);
            _damageToDeal = damageToDeal ?? new DamageToDeal(0);
        }

        internal void add(PowerAmount damageAmount) {
            switch (damageAmount) {
                case DamageToDeal deal:
                    _damageToDeal.change(deal);
                    break;
                case DamageToReceive receive:
                    damageAmount.change(receive);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported damage type: {damageAmount.GetType().Name}");
            }

            _damageToReceive.change(damageAmount);
        }

        internal DamageToReceive getDamageToReceive() {
            return _damageToReceive;
        }

        internal DamageToDeal getDamageToDeal() {
            return _damageToDeal;
        }
    }
}