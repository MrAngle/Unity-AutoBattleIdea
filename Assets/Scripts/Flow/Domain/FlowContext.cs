using MageFactory.CombatContext.Contract;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly ICombatCharacterEquippedItem placedEntryPoint;
        private int stepIndex;

        internal FlowContext(ICombatCharacterEquippedItem placedEntryPoint) {
            this.placedEntryPoint = placedEntryPoint;
        }

        internal ICombatCharacterEquippedItem getPlacedEntryPoint() {
            return placedEntryPoint;
        }

        internal void nextStep() {
            stepIndex++;
        }
    }
}