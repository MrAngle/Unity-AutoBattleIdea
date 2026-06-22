using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.ActionEffect.PredefinedOperations {
    public sealed class AddGuardPower : IOperation {
        private readonly PowerAmount guardPowerAmount;

        public AddGuardPower(PowerAmount guardPowerAmount) {
            this.guardPowerAmount = NullGuard.NotNullOrThrow(guardPowerAmount);
        }

        public PowerAmount getGuardPowerAmount() {
            return guardPowerAmount;
        }

        public void apply(IActionCapabilities actionCapabilities) {
            actionCapabilities.command().addGuardPower(guardPowerAmount);
        }
    }
}