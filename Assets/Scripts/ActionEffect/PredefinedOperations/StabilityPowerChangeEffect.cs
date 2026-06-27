using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.ActionEffect.PredefinedOperations {
    public sealed class AddStabilityPower : IOperation {
        private readonly PowerAmount stabilityPowerAmount;

        public AddStabilityPower(PowerAmount stabilityPowerAmount) {
            this.stabilityPowerAmount = NullGuard.NotNullOrThrow(stabilityPowerAmount);
        }

        public PowerAmount getStabilityPowerAmount() {
            return stabilityPowerAmount;
        }

        public void apply(IActionCapabilities actionCapabilities) {
            actionCapabilities.command().addStabilityPower(stabilityPowerAmount);
        }
    }
}