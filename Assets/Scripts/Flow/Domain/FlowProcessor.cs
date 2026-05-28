using System;
using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Flow.Api;
using MageFactory.Flow.Configuration;
using MageFactory.Flow.Contract;
using MageFactory.Flow.Domain.ActionCapability;
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

        private IFlowItem currentItem;
        private readonly CurrentFlowItemCast currentItemCast = new();
        private bool finished;

        private FlowProcessor(
            FlowProcessingCapabilities flowProcessingCapabilities,
            IFlowItem startNode,
            FlowProcessorSettings settings
        ) {
            currentItem = NullGuard.NotNullOrThrow(startNode);
            this.flowProcessingCapabilities = NullGuard.NotNullOrThrow(flowProcessingCapabilities);
            this.settings = NullGuard.NotNullOrThrow(settings);
            currentItemCast.startCasting(currentItem, settings.getCastTimeMode());
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
            FlowContext context = new FlowContext(flowKind, startNode, flowConsumer, flowOwner, flowRouter);
            FlowProcessingCapabilities flowProcessingCapabilities =
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

            if (combatTicks.isNegative()) {
                throw new ArgumentOutOfRangeException(
                    nameof(combatTicks),
                    combatTicks,
                    "Combat ticks cannot be negative.");
            }

            if (combatTicks.isZero()) {
                return;
            }

            advanceFlowDuring(combatTicks);
        }

        private void advanceFlowDuring(CombatTicks combatTicks) {
            CombatTicks availableTicks = combatTicks;
            int processedItemsThisTick = 0;

            while (canProcessMoreItems(processedItemsThisTick)) {
                if (!tryFinishCurrentItemCast(ref availableTicks)) {
                    return;
                }

                executeCurrentItemEffects();
                processedItemsThisTick++;

                if (!tryMoveToNextItem()) {
                    return;
                }

                if (mustWaitForMoreTicks(availableTicks)) {
                    return;
                }
            }
        }

        private bool canProcessMoreItems(int processedItemsThisTick) {
            return currentItem != null && processedItemsThisTick < settings.getMaxStepsPerSlice();
        }

        private bool tryFinishCurrentItemCast(ref CombatTicks availableTicks) {
            return currentItemCast.tryFinishCasting(ref availableTicks);
        }

        private bool mustWaitForMoreTicks(CombatTicks availableTicks) {
            return availableTicks <= CombatTicks.ZERO && currentItemCast.isCasting();
        }

        private void executeCurrentItemEffects() {
            IActionDescription actionDescription =
                flowProcessingCapabilities.query().prepareActionDescription(currentItem);

            ActionCapabilities actionCapabilities =
                flowProcessingCapabilities.query().prepareActionCapabilities(currentItem);

            IReadOnlyList<IOperation> effects = actionDescription
                .getEffectsDescriptor()
                .getEffects();

            for (int i = 0; i < effects.Count; i++) {
                effects[i].apply(actionCapabilities);
            }

            visitedNodeIds.Add(currentItem.getId());
        }

        private bool tryMoveToNextItem() {
            if (flowProcessingCapabilities.query()
                .tryFindNextNode(currentItem, visitedNodeIds, out IFlowItem nextItem)) {
                currentItem = nextItem;
                currentItemCast.startCasting(currentItem, settings.getCastTimeMode());
                return true;
            }

            finishFlow();
            currentItem = null;
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