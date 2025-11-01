
using Inventory.EntryPoints;
using UnityEngine;

namespace Combat.Flow.Domain.Aggregate
{
    /// Kanał przepływu – damage/defense itd.
    public enum FlowKind
    {
        Damage,
        Defense
    }

    public class FlowSeed {
        private readonly long _initPower;
        
        public long Power() {
            return _initPower;
        }

        public FlowSeed(long power) {
            _initPower = power;
        }
    }
    
    public class FlowPayload
    {
        public long Power { get; private set; }

        public FlowPayload(long power) => Power = power;
        
        public void Add(long amount) => Power += amount;

    }

    public class FlowContext
    {
        public IPlacedEntryPoint PlacedEntryPoint { get; }

        public int StepIndex { get; private set; }

        public FlowContext(IPlacedEntryPoint placedEntryPoint) {
            PlacedEntryPoint = placedEntryPoint;
        }

        public void NextStep() => StepIndex++;
    }
}