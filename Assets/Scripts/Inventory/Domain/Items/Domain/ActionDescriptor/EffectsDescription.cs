using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Inventory.Api;

namespace MageFactory.Inventory.Domain {
    // odpowiednik
    // public sealed class ActionCommandDescriptor : IActionDescriptor {
    //     private readonly IReadOnlyList<IEffectDescriptor> _effects;
    //
    //     public ActionCommandDescriptor(params IEffectDescriptor[] effects) {
    //         _effects = effects;
    //     }
    //
    //     public IActionCommand ToActionCommand(IFlowContext flowContext) {
    //         return new ActionCommand(flowContext, _effects);
    //     }
    // }

    public sealed class EffectsDescription : IEffectsDescriptor {
        private readonly IReadOnlyList<IEffect> _effects;

        public EffectsDescription(params IEffect[] effects) {
            _effects = effects;
        }

        public IReadOnlyList<IEffect> getEffects() {
            return _effects;
        }

        // public IActionCommand ToActionCommand(IFlowContext flowContext) {
        //     return new ActionCommand(flowContext, _effects);
        // }
    }
}