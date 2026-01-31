using System.Collections.Generic;
using Contracts.Flow;

namespace Contracts.Actionexe {
    public sealed class ActionCommandDescriptor : IActionDescriptor {
        private readonly IReadOnlyList<IEffectDescriptor> _effects;

        public ActionCommandDescriptor(params IEffectDescriptor[] effects) {
            _effects = effects;
        }

        public IActionCommand ToActionCommand(IFlowContext flowContext) {
            return new ActionCommand(flowContext, _effects);
        }
    }
}