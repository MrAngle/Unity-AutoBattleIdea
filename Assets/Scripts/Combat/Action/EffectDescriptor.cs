using Combat.Flow.Domain.Aggregate;

namespace UI.Combat.Action {
    public interface IEffectDescriptor
    {
        // IEffectInstance Bind(IFlow flowAggregate);
        void Execute(IFlowContext flow);
    }
    
    public sealed class AddPower : IEffectDescriptor {
        private long _amount;
        public AddPower(long amount) {
            _amount = amount;
        }

        public void Execute(IFlowContext flow) {
            flow.AddPower(_amount);
        }

        // public IEffectInstance Bind(FlowAggregate aggregate)
        //     => new AddPowerEffectInstance(aggregate, Amount);
    }
    
    // public interface IEffectInstance {
    //     void Execute();
    // }

    // public sealed class AddPowerEffectInstance : IEffectInstance
    // {
    //     private readonly FlowAggregate _flow;
    //     private readonly long _amount;
    //
    //     public AddPowerEffectInstance(FlowAggregate flow, long amount)
    //         => (_flow, _amount) = (flow, amount);
    //
    //     public void Execute() => _flow.AddPower(_amount);
    // }
}