using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.Flow.Domain.FlowCapability;
using MageFactory.Flow.Domain.Service;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Flow.Domain {
    internal class FlowProcessor : IFlowProcessor {
        private readonly IActionExecutor actionExecutor;
        private readonly FlowProcessingCapabilities flowProcessingCapabilities;
        private readonly List<Id<ItemId>> visitedNodeIds = new();

        private readonly IFlowStepScheduler stepScheduler;
        private readonly FlowProcessorSettings settings;

        private CancellationTokenSource cancellationTokenSource;
        private IFlowItem currentNode;
        private bool isRunning;

        private FlowProcessor(
            FlowProcessingCapabilities flowProcessingCapabilities,
            IFlowItem startNode,
            IActionExecutor actionExecutor,
            IFlowStepScheduler stepScheduler,
            FlowProcessorSettings settings
        ) {
            currentNode = NullGuard.NotNullOrThrow(startNode);
            this.flowProcessingCapabilities = NullGuard.NotNullOrThrow(flowProcessingCapabilities);
            this.actionExecutor = NullGuard.NotNullOrThrow(actionExecutor);
            this.stepScheduler = NullGuard.NotNullOrThrow(stepScheduler);
            this.settings = NullGuard.NotNullOrThrow(settings);

            visitedNodeIds.Clear();
        }

        internal static IFlowProcessor create(
            IFlowItem startNode,
            IFlowRouter flowRouter,
            IActionExecutor actionExecutor,
            IFlowConsumer flowConsumer,
            IFlowOwner flowOwner,
            IFlowCapabilities flowCapabilities,
            ActionContextFactory actionContextFactory,
            IFlowStepScheduler stepScheduler = null,
            FlowProcessorSettings settings = null
        ) {
            var context = new FlowContext(startNode, flowConsumer, flowOwner, flowRouter);
            var flowProcessingCapabilities =
                new FlowProcessingCapabilities(context, actionContextFactory, flowCapabilities);

            return new FlowProcessor(
                flowProcessingCapabilities,
                startNode,
                actionExecutor,
                stepScheduler ?? new TaskYieldFlowStepScheduler(),
                settings ?? new FlowProcessorSettings()
            );
        }

        public void start() {
            _ = startAsync();
        }

        private Task startAsync() {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            return stepLoopAsync(cancellationTokenSource.Token);
        }

        private async Task stepLoopAsync(CancellationToken ct) {
            if (isRunning) return;
            isRunning = true;

            try {
                var stepsInSlice = 0;

                while (!ct.IsCancellationRequested && currentNode != null) {
                    await processAsync(ct);

                    if (ct.IsCancellationRequested) break;

                    if (!goNext()) break;

                    stepsInSlice++;
                    if (stepsInSlice >= settings.maxStepsPerSlice) {
                        stepsInSlice = 0;
                        await stepScheduler.yieldAsync(ct);
                    }
                }
            }
            finally {
                isRunning = false;
            }
        }

        private async Task processAsync(CancellationToken cancellationToken) {
            ExecuteActionCommand executeActionCommand =
                flowProcessingCapabilities.query().prepareExecuteActionCommand(currentNode);

            try {
                await actionExecutor.executeAsync(executeActionCommand);
            }
            catch (OperationCanceledException) {
                throw;
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                throw;
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
            return false;
        }

        public void finishFlow() {
            flowProcessingCapabilities.command().consumeFlow();
        }

        public void stop() {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }
}