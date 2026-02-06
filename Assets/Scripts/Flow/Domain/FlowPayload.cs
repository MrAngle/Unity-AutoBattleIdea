using System;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;

namespace MageFactory.Flow.Domain {
    /// Kanał przepływu – damage/defense itd.
    public class FlowSeed {
        private readonly long _initPower;

        public FlowSeed(long power) {
            _initPower = power;
        }

        public long Power() {
            return _initPower;
        }
    }

    public class FlowPayload {
        private readonly DamageToDeal _damageToDeal;
        private readonly DamageToReceive _damageToReceive;

        public FlowPayload(long power, DamageToReceive damageToReceiveToReceive = null,
            DamageToDeal damageToDeal = null) {
            _damageToReceive = damageToReceiveToReceive ?? new DamageToReceive(0);
            _damageToDeal = damageToDeal ?? new DamageToDeal(0);
        }

        public void Add(PowerAmount damageAmount) {
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

        // public void Add(DamageToDeal damageToDeal) {
        //     _damageToReceive.change(damageToDeal);
        // }


        public DamageToReceive GetDamageToReceive() {
            return _damageToReceive;
        }

        public DamageToDeal GetDamageToDeal() {
            return _damageToDeal;
        }
    }

    public class FlowContext {
        public FlowContext(IPlacedEntryPoint placedEntryPoint) {
            PlacedEntryPoint = placedEntryPoint;
        }

        public IPlacedEntryPoint PlacedEntryPoint { get; }

        public int StepIndex { get; private set; }

        public void NextStep() {
            StepIndex++;
        }
    }
}