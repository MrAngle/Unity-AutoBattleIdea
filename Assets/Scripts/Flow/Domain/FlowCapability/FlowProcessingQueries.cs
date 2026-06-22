using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Flow.Contract;
using MageFactory.Flow.Domain.ActionCapability;
using MageFactory.Flow.Domain.Service;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowProcessingQueries {
        private readonly FlowContext flowContext;
        private readonly ActionContextFactory actionContextFactory;
        private readonly IFlowCapabilities flowCapabilities;

        public FlowProcessingQueries(
            FlowContext flowContext,
            ActionContextFactory actionContextFactory,
            IFlowCapabilities flowCapabilities
        ) {
            this.flowContext = NullGuard.NotNullOrThrow(flowContext);
            this.actionContextFactory = NullGuard.NotNullOrThrow(actionContextFactory);
            this.flowCapabilities = NullGuard.NotNullOrThrow(flowCapabilities);
        }

        internal bool tryFindNextNode(
            IFlowItem sourceNode,
            IReadOnlyCollection<Id<ItemId>> nodeIdsToIgnore,
            out IFlowItem nextNode) {
            nextNode = flowContext.getFlowRouter().decideNext(sourceNode, nodeIdsToIgnore);
            return nextNode != null;
        }

        internal IActionDescription prepareActionDescription(IFlowItem actionItemInvoker) {
            return actionItemInvoker.prepareItemActionDescription();
        }

        internal ActionCapabilities prepareActionCapabilities(IFlowItem actionItemInvoker) {
            // maybe it may be optimized by caching/memoization
            ActionContext actionContext = actionContextFactory.create(flowContext, actionItemInvoker);

            ActionCapabilities actionCapabilities = new ActionCapabilities(actionContext, flowCapabilities);
            return actionCapabilities;
        }

        internal bool isOutputPort(IFlowItem item) {
            return item is IFlowPortPlacedItem portPlacedItem
                   && portPlacedItem.getFlowPortKind() == FlowPortKind.Output;
        }

        internal bool isInputPort(IFlowItem item) {
            return item is IFlowPortPlacedItem portPlacedItem
                   && portPlacedItem.getFlowPortKind() == FlowPortKind.Input;
        }
    }
}