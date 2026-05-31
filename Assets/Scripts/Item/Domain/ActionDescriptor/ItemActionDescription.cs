using MageFactory.ActionEffect;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Item.Domain.ActionDescriptor {
    internal sealed class ItemActionDescription : IActionDescription {
        private readonly ItemCastTime castTime;
        private readonly IOperations operations;

        internal ItemActionDescription(
            ItemCastTime castTime,
            IOperations operations) {
            this.castTime = NullGuard.NotNullOrThrow(castTime);
            this.operations = NullGuard.NotNullOrThrow(operations);
        }

        public ItemCastTime getCastTime() {
            return castTime;
        }

        public IOperations getEffectsDescriptor() {
            return operations;
        }
    }
}