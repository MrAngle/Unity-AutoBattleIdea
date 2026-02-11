using System;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Domain {
    public class FlowPayload {
        private readonly DamageToDeal damageToDeal;
        private readonly DamageToReceive damageToReceive;

        internal FlowPayload(DamageToReceive damageToReceiveToReceive = null,
            DamageToDeal damageToDeal = null) {
            damageToReceive = damageToReceiveToReceive ?? new DamageToReceive(0);
            this.damageToDeal = damageToDeal ?? new DamageToDeal(0);
        }

        internal void add(PowerAmount damageAmount) {
            switch (damageAmount) {
                case DamageToDeal deal:
                    damageToDeal.change(deal);
                    break;
                case DamageToReceive receive:
                    damageAmount.change(receive);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported damage type: {damageAmount.GetType().Name}");
            }

            damageToReceive.change(damageAmount);
        }

        internal DamageToReceive getDamageToReceive() {
            return damageToReceive;
        }

        internal DamageToDeal getDamageToDeal() {
            return damageToDeal;
        }
    }
}