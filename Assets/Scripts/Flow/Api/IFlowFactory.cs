using System;
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
        private readonly PowerAmount initialAttackPower;
        private readonly Id<CharacterId> sourceCharacterId;

        public FlowCreationCommand(
            Id<ActiveFlowId> flowId,
            FlowKind flowKind,
            IFlowItem startNode,
            IFlowRouter router,
            IFlowConsumer flowConsumer,
            IFlowCapabilities flowCapabilities,
            IFlowOwner flowOwner) :
            this(
                flowId,
                flowKind,
                startNode,
                router,
                flowConsumer,
                flowCapabilities,
                flowOwner,
                PowerAmount.noPower(),
                default) {
        }

        public FlowCreationCommand(
            Id<ActiveFlowId> flowId,
            FlowKind flowKind,
            IFlowItem startNode,
            IFlowRouter router,
            IFlowConsumer flowConsumer,
            IFlowCapabilities flowCapabilities,
            IFlowOwner flowOwner,
            PowerAmount initialAttackPower,
            Id<CharacterId> sourceCharacterId) {
            this.flowId = NullGuard.ValidIdOrThrow(flowId);
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
            this.startNode = NullGuard.NotNullOrThrow(startNode);
            this.router = NullGuard.NotNullOrThrow(router);
            this.flowConsumer = NullGuard.NotNullOrThrow(flowConsumer);
            this.flowCapabilities = NullGuard.NotNullOrThrow(flowCapabilities);
            this.flowOwner = NullGuard.NotNullOrThrow(flowOwner);
            this.initialAttackPower = NullGuard.NotNullOrThrow(initialAttackPower);
            this.sourceCharacterId = sourceCharacterId;

            if (sourceCharacterId.Value < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(sourceCharacterId),
                    sourceCharacterId,
                    "Source character id cannot be negative.");
            }
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

        public PowerAmount getInitialAttackPower() {
            return initialAttackPower;
        }

        public bool hasSourceCharacterId() {
            return sourceCharacterId.Value > 0;
        }

        public Id<CharacterId> getSourceCharacterId() {
            return sourceCharacterId;
        }
    }

    public interface IFlowFactory {
        IFlowProcessor create(FlowCreationCommand command);
    }
}