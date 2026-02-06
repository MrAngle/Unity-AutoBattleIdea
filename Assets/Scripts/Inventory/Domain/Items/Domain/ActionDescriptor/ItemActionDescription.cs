using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Inventory.Domain {
    public sealed class ItemActionDescription : IItemActionDescription {
        private readonly Duration castTime;
        private readonly IEffectsDescriptor effectsDescriptor;

        public ItemActionDescription(Duration castTime, IEffectsDescriptor effectsDescriptor) {
            this.castTime = castTime;
            this.effectsDescriptor = effectsDescriptor;
            NullGuard.NotNullCheckOrThrow(this.castTime, this.effectsDescriptor);
        }

        public Duration getCastTime() {
            return castTime;
        }

        public IEffectsDescriptor getEffectsDescriptor() {
            return effectsDescriptor;
        }
    }
}