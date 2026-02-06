// using System;
// using System.Collections.Generic;
// using System.Linq;
// using MageFactory.ActionEffect;
// using MageFactory.Flow.Api;
// using MageFactory.Shared.Utility;
//
// namespace MageFactory.ActionExecutor.Domain {
//     public sealed class ActionCommand /*: IActionCommand*/ {
//         private readonly IReadOnlyList<IEffect> _effects;
//         private readonly IFlowContext _flowContext;
//
//         public ActionCommand(IFlowContext flowContext, IEnumerable<IEffect> effects) {
//             _effects = (effects ?? throw new ArgumentNullException(nameof(effects))).ToList().AsReadOnly();
//             _flowContext = NullGuard.NotNullOrThrow(flowContext);
//         }
//
//         public void Execute() {
//             for (var i = 0; i < _effects.Count; i++) _effects[i].apply(_flowContext);
//         }
//     }
// }

