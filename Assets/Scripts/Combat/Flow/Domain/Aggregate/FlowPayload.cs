
namespace Combat.Flow.Domain.Aggregate
{
    /// Kanał przepływu – damage/defense itd.
    public enum FlowKind
    {
        Damage,
        Defense
    }

    public class FlowSeed
    {
        public long Power { get; }

        public FlowSeed(long power) => Power = power;

    }
    
    public class FlowPayload
    {
        public long Power { get; private set; }

        public FlowPayload(long power) => Power = power;
        
        public void Add(long amount) => Power += amount;

    }

    public class FlowContext
    {
        public FlowKind Kind { get; }
        public string SourceId { get; }
        public int StepIndex { get; private set; }

        public FlowContext(FlowKind kind, string sourceId)
        {
            Kind = kind;
            SourceId = sourceId;
            StepIndex = 0;
        }

        public void NextStep() => StepIndex++;
    }
}