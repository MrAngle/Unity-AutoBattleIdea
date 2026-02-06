using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public sealed class AddPower : IEffect {
        private readonly PowerAmount damageAmount;

        public AddPower(PowerAmount damageAmount) {
            this.damageAmount = damageAmount;
        }

        // public void Execute(IFlowContext flow) {
        //     flow.AddPower(_amount);
        // }
        public void apply(IEffectContext effectContext) {
            effectContext.addPower(damageAmount);
        }
    }
}