using System.Collections.Generic;
using MageFactory.ActionEffect;

namespace MageFactory.Item.Domain.ActionDescriptor {
    internal sealed class ItemOperationsDescription : IOperations {
        private readonly IReadOnlyList<IOperation> _effects;

        internal ItemOperationsDescription(params IOperation[] effects) {
            _effects = effects;
        }

        public IReadOnlyList<IOperation> getEffects() {
            return _effects;
        }
    }
}