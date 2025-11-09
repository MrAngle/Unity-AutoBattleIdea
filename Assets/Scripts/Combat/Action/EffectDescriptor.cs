using Combat.Flow.Domain.Aggregate;
using Combat.Flow.Domain.Shared;

namespace UI.Combat.Action {
    public interface IEffectDescriptor
    {
        // IEffectInstance Bind(IFlow flowAggregate);
        void Execute(IFlowContext flow);
    }
    
    public sealed class AddPower : IEffectDescriptor {
        private DamageAmount _amount;
        public AddPower(DamageAmount damageAmount) {
            _amount = damageAmount;
        }

        public void Execute(IFlowContext flow) {
            flow.AddPower(_amount);
        }
    }
    
    // public sealed class AddPower<TDamage> : IEffectDescriptor 
    //     where TDamage : DamageAmount
    // {
    //     private TDamage _amount;
    //     public AddPower(long amount) {
    //         switch (expression) {
    //             
    //         }
    //         
    //         
    //         _amount = new TDamage(amount);
    //     }
    //
    //     public void Execute(IFlowContext flow) {
    //         flow.AddPower( _amount);
    //     }
    // }
    
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