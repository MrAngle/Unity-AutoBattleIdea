using MageFactory.ActionEffect;
using MageFactory.Character.Contract.Event;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.ActionCapability {
    internal class ActionCommandBus : IActionCommandBus {
        private readonly ActionContext actionContext;
        private readonly IFlowCapabilities flowCapabilities;

        public ActionCommandBus(ActionContext actionContext, IFlowCapabilities flowCapabilities) {
            this.actionContext = NullGuard.NotNullOrThrow(actionContext);
            this.flowCapabilities = NullGuard.NotNullOrThrow(flowCapabilities);
        }

        public void addPower(DamageRole damageRole, PowerAmount powerAmount) {
            actionContext.addPower(damageRole, powerAmount);

            IFlowItem actionItemInvoker = actionContext.getActionItemInvoker();
            actionContext.getSignalBus()
                .Fire(new ItemPowerChangedDtoEvent(actionItemInvoker.getId(), powerAmount.getPower()));
        }

        public void pushRightAdjacentItemRight() {
            flowCapabilities.command().tryMoveRightAdjacentItemToRight(actionContext.getActionItemInvoker());
        }
    }
}