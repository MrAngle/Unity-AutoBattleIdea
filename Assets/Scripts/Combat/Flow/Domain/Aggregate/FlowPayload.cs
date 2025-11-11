using System;
using Combat.Flow.Domain.Shared;
using Inventory.EntryPoints;

namespace Combat.Flow.Domain.Aggregate {
    /// Kanał przepływu – damage/defense itd.
    public enum FlowKind {
        Damage,
        Defense
    }

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
        private DamageToReceive _damageToReceive;
        private DamageToDeal _damageToDeal;

        public FlowPayload(long power, DamageToReceive damageToReceiveToReceive = null, DamageToDeal damageToDeal = null) {
            _damageToReceive = damageToReceiveToReceive ?? new DamageToReceive(0);
            _damageToDeal = damageToDeal ?? new DamageToDeal(0);
        }

        public void Add(DamageAmount damageAmount) {
            switch (damageAmount)
            {
                case DamageToDeal deal:
                    _damageToDeal.Add(deal);
                    break;
                case DamageToReceive receive:
                    damageAmount.Add(receive);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported damage type: {damageAmount.GetType().Name}");
            }
            
            _damageToReceive.Add(damageAmount);
        }
        
        public void Add(DamageToDeal damageToDeal) {
            _damageToReceive.Add(damageToDeal);
        }
        

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