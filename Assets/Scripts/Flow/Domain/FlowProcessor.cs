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
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Flow.Domain {
    internal class FlowProcessor : IFlowProcessor {
        private readonly IActionExecutor actionExecutor;
        private readonly FlowProcessingCapabilities flowProcessingCapabilities;
        private readonly List<long> visitedNodeIds = new();

        private CancellationTokenSource cancellationTokenSource;
        private IFlowItem currentNode;
        private bool isRunning;

        private FlowProcessor(FlowProcessingCapabilities flowProcessingCapabilities, IFlowItem startNode,
                              IActionExecutor actionExecutor) {
            currentNode = NullGuard.NotNullOrThrow(startNode);
            this.flowProcessingCapabilities = NullGuard.NotNullOrThrow(flowProcessingCapabilities);
            this.actionExecutor = NullGuard.NotNullOrThrow(actionExecutor);

            visitedNodeIds.Clear(); // shouldn't be needed
        }

        internal static IFlowProcessor create(IFlowItem startNode,
                                              IFlowRouter flowRouter,
                                              IActionExecutor actionExecutor,
                                              IFlowConsumer flowConsumer,
                                              IFlowOwner flowOwner,
                                              IFlowCapabilities flowCapabilities,
                                              ActionContextFactory actionContextFactory) {
            var context = new FlowContext(startNode, flowConsumer, flowOwner, flowRouter);
            var flowProcessingCapabilities =
                new FlowProcessingCapabilities(context, actionContextFactory, flowCapabilities);

            return new FlowProcessor(flowProcessingCapabilities, startNode, actionExecutor);
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
                while (!ct.IsCancellationRequested && currentNode != null) {
                    await processAsync(ct);
                    if (ct.IsCancellationRequested) break;
                    if (!await goNextAsync(ct)) break;
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
                throw; // for now
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                throw; // for now
            }

            visitedNodeIds.Add(currentNode.getId());
        }

        private async Task<bool> goNextAsync(CancellationToken cancellationToken) {
            if (flowProcessingCapabilities.query()
                .tryFindNextNode(currentNode, visitedNodeIds, out IFlowItem nextNode)) {
                currentNode = nextNode;
            }
            else {
                finishFlow();
                currentNode = null;
                return false;
            }

            await Task.Yield();
            return true;
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