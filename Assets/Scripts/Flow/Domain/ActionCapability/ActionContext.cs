using MageFactory.Flow.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using Zenject;

namespace MageFactory.Flow.Domain.ActionCapability {
    internal class ActionContext {
        private readonly FlowContext flowContext;
        private readonly IFlowItem actionItemInvoker;

        private readonly SignalBus signalBus; // probably it shouldnt be here

        internal ActionContext(FlowContext flowContext, IFlowItem actionItemInvoker, SignalBus signalBus) {
            this.flowContext = NullGuard.NotNullOrThrow(flowContext);
            this.actionItemInvoker = NullGuard.NotNullOrThrow(actionItemInvoker);

            this.signalBus = NullGuard.NotNullOrThrow(signalBus);
        }

        internal IFlowItem getActionItemInvoker() {
            return actionItemInvoker;
        }

        internal SignalBus getSignalBus() {
            return signalBus;
        }

        internal void addPower(PowerAmount damageAmount) {
            flowContext.getFlowPayload().add(damageAmount);
        }
    }
}