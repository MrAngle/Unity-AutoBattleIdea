using System.Collections.Generic;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Flow.Contract;
using MageFactory.Flow.Domain.ActionCapability;
using MageFactory.Flow.Domain.Service;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain.FlowCapability {
    internal class FlowQueries {
        private readonly FlowContext flowContext;
        private readonly ActionContextFactory actionContextFactory;

        public FlowQueries(FlowContext flowContext, ActionContextFactory actionContextFactory) {
            this.flowContext = NullGuard.NotNullOrThrow(flowContext);
            this.actionContextFactory = NullGuard.NotNullOrThrow(actionContextFactory);
        }

        internal bool tryFindNextNode(IFlowItem sourceNode, List<long> nodeIdsToIgnore, out IFlowItem nextNode) {
            nextNode = flowContext.getFlowRouter().decideNext(sourceNode, nodeIdsToIgnore);
            return nextNode != null;
        }

        internal ExecuteActionCommand prepareExecuteActionCommand(IFlowItem actionItemInvoker) {
            var actionSpecification = actionItemInvoker.prepareItemActionDescription();
            return new ExecuteActionCommand(actionSpecification, prepareActionCapabilities(actionItemInvoker));
        }

        private ActionCapabilities prepareActionCapabilities(IFlowItem actionItemInvoker) {
            // maybe it may be optimized by caching/memoization
            ActionContext actionContext = actionContextFactory.create(flowContext, actionItemInvoker);

            ActionCapabilities actionCapabilities = new ActionCapabilities(actionContext);
            return actionCapabilities;
        }
    }
}