using System;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain {
    public class FlowPower {
        private readonly FlowKind flowKind;


        private readonly DamageToDeal damageToDeal;
        private readonly DamageToReceive damageToReceive;

        internal FlowPower(FlowKind flowKind, DamageToReceive damageToReceiveToReceive = null,
                           DamageToDeal damageToDeal = null) {
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
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