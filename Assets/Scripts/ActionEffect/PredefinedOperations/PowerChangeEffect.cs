using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect.PredefinedOperations {
    public sealed class AddPower : IOperation {
        private readonly PowerAmount damageAmount;

        public AddPower(PowerAmount damageAmount) {
            this.damageAmount = damageAmount;
        }

        public PowerAmount getDamageAmount() {
            return damageAmount;
        }

        public void apply(IActionCapabilities actionCapabilities) {
            actionCapabilities.command().addPower(damageAmount);
        }
    }
}