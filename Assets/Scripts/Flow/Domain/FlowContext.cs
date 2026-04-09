using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly FlowKind flowKind;
        private readonly FlowPower flowPower;
        private readonly IFlowItem startEntryPoint;
        private readonly IFlowConsumer flowConsumer;
        private readonly IFlowOwner flowOwner;
        private readonly IFlowRouter router;

        private int stepIndex;

        internal FlowContext(FlowKind flowKind, IFlowItem startEntryPoint, IFlowConsumer flowConsumer,
                             IFlowOwner flowOwner,
                             IFlowRouter router) {
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
            this.startEntryPoint = NullGuard.NotNullOrThrow(startEntryPoint);
            this.flowConsumer = NullGuard.NotNullOrThrow(flowConsumer);
            this.flowOwner = NullGuard.NotNullOrThrow(flowOwner);
            this.router = NullGuard.NotNullOrThrow(router);
            flowPower = new FlowPower(this.flowKind);
            NullGuard.NotNullCheckOrThrow(this.startEntryPoint, this.flowConsumer, this.flowOwner, flowPower,
                this.router);
        }

        internal IFlowConsumer getFlowConsumer() {
            return flowConsumer;
        }

        internal IFlowOwner getFlowOwner() {
            return flowOwner;
        }

        internal IFlowRouter getFlowRouter() {
            return router;
        }

        internal void addPower(PowerAmount damageAmount) {
            flowPower.add(damageAmount);
        }

        internal FlowKind getFlowKind() {
            return flowKind;
        }

        internal PowerAmount getDamagePower() {
            return flowPower.getDamageToDeal();
        }
    }
}