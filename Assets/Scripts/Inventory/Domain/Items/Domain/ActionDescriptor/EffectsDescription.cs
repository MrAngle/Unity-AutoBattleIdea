using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Api;

namespace MageFactory.Inventory.Domain {
    public sealed class EffectsDescription : IEffectsDescriptor {
        private readonly IReadOnlyList<IEffect> _effects;

        public EffectsDescription(params IEffect[] effects) {
            _effects = effects;
        }

        public IReadOnlyList<IEffect> getEffects() {
            return _effects;
        }
    }
}