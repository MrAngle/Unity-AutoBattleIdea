using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Api {
    public sealed class FlowCreationCommand {
        private readonly Id<ActiveFlowId> flowId;
        private readonly FlowKind flowKind;
        private readonly IFlowItem startNode;
        private readonly IFlowRouter router;
        private readonly IFlowConsumer flowConsumer;
        private readonly IFlowCapabilities flowCapabilities;
        private readonly IFlowOwner flowOwner;

        public FlowCreationCommand(
            Id<ActiveFlowId> flowId,
            FlowKind flowKind,
            IFlowItem startNode,
            IFlowRouter router,
            IFlowConsumer flowConsumer,
            IFlowCapabilities flowCapabilities,
            IFlowOwner flowOwner) {
            this.flowId = NullGuard.ValidIdOrThrow(flowId);
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
            this.startNode = NullGuard.NotNullOrThrow(startNode);
            this.router = NullGuard.NotNullOrThrow(router);
            this.flowConsumer = NullGuard.NotNullOrThrow(flowConsumer);
            this.flowCapabilities = NullGuard.NotNullOrThrow(flowCapabilities);
            this.flowOwner = NullGuard.NotNullOrThrow(flowOwner);
        }

        public Id<ActiveFlowId> getFlowId() {
            return flowId;
        }

        public FlowKind getFlowKind() {
            return flowKind;
        }

        public IFlowItem getStartNode() {
            return startNode;
        }

        public IFlowRouter getRouter() {
            return router;
        }

        public IFlowConsumer getFlowConsumer() {
            return flowConsumer;
        }

        public IFlowCapabilities getFlowCapabilities() {
            return flowCapabilities;
        }

        public IFlowOwner getFlowOwner() {
            return flowOwner;
        }
    }

    public interface IFlowFactory {
        IFlowProcessor create(FlowCreationCommand command);
    }
}