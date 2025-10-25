
namespace Combat.Flow.Domain.Aggregate
{
    /// Kanał przepływu – damage/defense itd.
    public enum FlowKind
    {
        Damage,
        Defense
        // w przyszłości: Mana/Energy
    }

    internal class FlowSeed
    {
        public long Power { get; }

        public FlowSeed(long power) => Power = power;

    }
    
    internal class FlowPayload
    {
        public long Power { get; private set; }

        public FlowPayload(long power) => Power = power;
        
        public void Add(long amount) => Power += amount;

    }

    /// Kontekst przepływu – metadane niezależne od payloadu.
    internal class FlowContext
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

    /// Model przekazywany między krokami – łączy Payload + Context.
    internal class FlowModel
    {
        public FlowSeed FlowSeed { get; }
        public FlowPayload FlowPayload { get; }
        public FlowContext FlowContext { get; }

        public FlowModel(FlowSeed flowSeed, FlowContext flowContext)
        {
            FlowSeed = flowSeed;
            FlowPayload = new FlowPayload(flowSeed.Power);
            FlowContext = flowContext;
        }
    }
}