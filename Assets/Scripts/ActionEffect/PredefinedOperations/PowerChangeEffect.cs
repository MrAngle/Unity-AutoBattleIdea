using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;

namespace MageFactory.ActionEffect.PredefinedOperations {
    public sealed class AddPower : IOperation {
        private readonly PowerAmount damageAmount;
        private readonly DamageRole damageRole;

        public AddPower(DamageRole damageRole, PowerAmount damageAmount) {
            this.damageAmount = damageAmount;
            this.damageRole = damageRole;
        }

        public PowerAmount getDamageAmount() {
            return damageAmount;
        }

        public void apply(IActionCapabilities actionCapabilities) {
            actionCapabilities.command().addPower(damageRole, damageAmount);
        }
    }
}