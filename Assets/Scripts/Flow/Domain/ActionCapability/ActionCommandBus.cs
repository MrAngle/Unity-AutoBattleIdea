using System;
using MageFactory.ActionEffect;
using MageFactory.Character.Contract.Event;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.ActionCapability {
    internal class ActionCommandBus : IActionCommandBus {
        private readonly ActionContext actionContext;

        public ActionCommandBus(ActionContext actionContext) {
            this.actionContext = NullGuard.NotNullOrThrow(actionContext);
        }

        public void addPower(PowerAmount powerAmount) {
            actionContext.addPower(powerAmount);

            IFlowItem actionItemInvoker = actionContext.getActionItemInvoker();
            actionContext.getSignalBus()
                .Fire(new ItemPowerChangedDtoEvent(actionItemInvoker.getId(), powerAmount.getPower()));
        }

        public void pushRightAdjacentItemRight() {
            throw new NotImplementedException();
        }
    }
}