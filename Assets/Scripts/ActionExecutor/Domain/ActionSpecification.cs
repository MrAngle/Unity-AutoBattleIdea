// using MageFactory.Flow.Api;
// using MageFactory.Shared.Model;
// using MageFactory.Shared.Utility;
//
// namespace MageFactory.ActionExecutor.Domain {
//     public sealed class ActionSpecification /*: IActionSpecification*/ {
//         private readonly ActionCommandDescriptor actionCommandDescriptor;
//         private readonly Duration castTime;
//
//         public ActionSpecification(Duration castTime, ActionCommandDescriptor actionCommandDescriptor) {
//             this.castTime = castTime;
//             this.actionCommandDescriptor = actionCommandDescriptor;
//             NullGuard.NotNullCheckOrThrow(this.castTime, this.actionCommandDescriptor);
//         }
//
//         public PreparedAction toPreparedAction(IFlowContext flowContext) {
//             return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
//         }
//     }
// }

