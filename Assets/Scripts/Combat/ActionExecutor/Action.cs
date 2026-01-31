namespace Combat.ActionExecutor {
    // public interface IActionCommand {
    //     void Execute();
    // }

    // public interface IPreparedAction {
    //     ActionTiming GetActionTiming();
    //     void Execute();
    // }
    //
    // public interface IActionSpecification {
    //     IPreparedAction ToPreparedAction(IFlowContext flowContext);
    // }
    //
    // public interface IActionDescriptor {
    //     IActionCommand ToActionCommand(IFlowContext flowContext);
    // }


    // public sealed class PreparedAction : IPreparedAction {
    //     private readonly IActionCommand _actionCommand;
    //     private readonly ActionTiming _actionTiming;
    //
    //     public PreparedAction(ActionTiming timing, IActionCommand command) {
    //         _actionTiming = timing;
    //         _actionCommand = command ?? throw new ArgumentNullException(nameof(command));
    //         NullGuard.NotNullCheckOrThrow(_actionTiming, _actionCommand);
    //     }
    //
    //     public ActionTiming GetActionTiming() {
    //         return _actionTiming;
    //     }
    //
    //     public void Execute() {
    //         _actionCommand.Execute();
    //     }
    // }

    // public sealed class ActionSpecification : IActionSpecification {
    //     private readonly IActionDescriptor _actionCommand;
    //     private readonly ActionTiming _actionTiming;
    //
    //     public ActionSpecification(ActionTiming actionTiming, IActionDescriptor actionCommand) {
    //         _actionTiming = actionTiming;
    //         _actionCommand = actionCommand;
    //         NullGuard.NotNullCheckOrThrow(_actionTiming, _actionCommand);
    //     }
    //
    //     public IPreparedAction ToPreparedAction(IFlowContext flowContext) {
    //         return new PreparedAction(_actionTiming, _actionCommand.ToActionCommand(flowContext));
    //     }
    // }

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

    // public sealed class ActionCommand : IActionCommand {
    //     private readonly IReadOnlyList<IEffectDescriptor> _effects;
    //     private readonly IFlowContext _flowContext;
    //
    //     public ActionCommand(IFlowContext flowContext, IEnumerable<IEffectDescriptor> effects) {
    //         _effects = (effects ?? throw new ArgumentNullException(nameof(effects))).ToList().AsReadOnly();
    //         _flowContext = NullGuard.NotNullOrThrow(flowContext);
    //     }
    //
    //     public void Execute() {
    //         for (var i = 0; i < _effects.Count; i++) _effects[i].Execute(_flowContext);
    //     }
    // }

    // public readonly struct ActionTiming {
    //     private readonly float _durationSeconds;
    //     
    //     public ActionTiming(float durationSeconds) {
    //         _durationSeconds = durationSeconds;
    //     }
    //     
    //     public float DurationSeconds() {
    //         return _durationSeconds;
    //     }
    // }
}