using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect {
    public sealed class AddPower : IOperation {
        private readonly PowerAmount damageAmount;

        public AddPower(PowerAmount damageAmount) {
            this.damageAmount = damageAmount;
        }

        public void apply(IActionContext actionContext) {
            actionContext.addPower(damageAmount);
        }
    }
}