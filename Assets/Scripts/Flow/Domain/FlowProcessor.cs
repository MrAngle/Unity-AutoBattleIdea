using System.Collections.Generic;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Flow.Api;
using MageFactory.Flow.Configuration;
using MageFactory.Flow.Contract;
using MageFactory.Flow.Domain.FlowCapability;
using MageFactory.Flow.Domain.Service;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain {
    internal class FlowProcessor : IFlowProcessor {
        private readonly FlowProcessingCapabilities flowProcessingCapabilities;
        private readonly List<Id<ItemId>> visitedNodeIds = new();
        private readonly FlowProcessorSettings settings;

        private IFlowItem currentNode;
        private bool finished;

        private FlowProcessor(
            FlowProcessingCapabilities flowProcessingCapabilities,
            IFlowItem startNode,
            FlowProcessorSettings settings
        ) {
            currentNode = NullGuard.NotNullOrThrow(startNode);
            this.flowProcessingCapabilities = NullGuard.NotNullOrThrow(flowProcessingCapabilities);
            this.settings = NullGuard.NotNullOrThrow(settings);
        }

        internal static IFlowProcessor create(
            FlowKind flowKind,
            IFlowItem startNode,
            IFlowRouter flowRouter,
            IFlowConsumer flowConsumer,
            IFlowOwner flowOwner,
            IFlowCapabilities flowCapabilities,
            ActionContextFactory actionContextFactory,
            FlowProcessorSettings settings
        ) {
            var context = new FlowContext(flowKind, startNode, flowConsumer, flowOwner, flowRouter);
            var flowProcessingCapabilities =
                new FlowProcessingCapabilities(context, actionContextFactory, flowCapabilities);

            return new FlowProcessor(
                flowProcessingCapabilities,
                startNode,
                NullGuard.NotNullOrThrow(settings)
            );
        }

        public void tick(CombatTicks combatTicks) {
            if (finished) {
                return;
            }

            var stepsProcessed = 0;

            while (currentNode != null && stepsProcessed < settings.getMaxStepsPerSlice()) {
                process();
                stepsProcessed++;

                if (!goNext()) {
                    break;
                }
            }
        }

        private void process() {
            ExecuteActionCommand executeActionCommand =
                flowProcessingCapabilities.query().prepareExecuteActionCommand(currentNode);

            var effects = executeActionCommand.itemActionDescription
                .getEffectsDescriptor()
                .getEffects();

            for (var i = 0; i < effects.Count; i++) {
                effects[i].apply(executeActionCommand.actionCapabilities);
            }

            visitedNodeIds.Add(currentNode.getId());
        }

        private bool goNext() {
            if (flowProcessingCapabilities.query()
                .tryFindNextNode(currentNode, visitedNodeIds, out IFlowItem nextNode)) {
                currentNode = nextNode;
                return true;
            }

            finishFlow();
            currentNode = null;
            finished = true;
            return false;
        }

        private void finishFlow() {
            flowProcessingCapabilities.command().consumeFlow();
        }

        public bool isFinished() {
            return finished;
        }
    }
}