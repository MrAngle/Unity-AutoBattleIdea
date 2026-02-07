using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Inventory.Domain {
    public sealed class ItemActionDescription : IActionDescription {
        private readonly Duration castTime;
        private readonly IOperations operations;

        public ItemActionDescription(Duration castTime, IOperations operations) {
            this.castTime = castTime;
            this.operations = operations;
            NullGuard.NotNullCheckOrThrow(this.castTime, this.operations);
        }

        public Duration getCastTime() {
            return castTime;
        }

        public IOperations getEffectsDescriptor() {
            return operations;
        }
    }
}