using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.Flow;
using MageFactory.Shared.Utility;

namespace Contracts.Actionexe {
    public sealed class ActionCommand : IActionCommand {
        private readonly IReadOnlyList<IEffectDescriptor> _effects;
        private readonly IFlowContext _flowContext;

        public ActionCommand(IFlowContext flowContext, IEnumerable<IEffectDescriptor> effects) {
            _effects = (effects ?? throw new ArgumentNullException(nameof(effects))).ToList().AsReadOnly();
            _flowContext = NullGuard.NotNullOrThrow(flowContext);
        }

        public void Execute() {
            for (var i = 0; i < _effects.Count; i++) _effects[i].Execute(_flowContext);
        }
    }
}