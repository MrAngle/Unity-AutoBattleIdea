using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Flow.Api;
using MageFactory.FlowRouting;
using MageFactory.Item.Controller.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.Flow.Domain {
    internal class FlowAggregate : IActionContext, IFlowAggregateFacade {
        private readonly IActionExecutor actionExecutor;
        private readonly FlowModel flowModel;
        private readonly IFlowRouter router;
        private readonly SignalBus signalBus;
        private readonly List<long> visitedNodeIds = new();

        private CancellationTokenSource cancellationTokenSource;
        private IPlacedItem currentNode;
        private bool isRunning;

        private FlowAggregate(FlowModel flowModel, IPlacedEntryPoint startNode, IFlowRouter flowRouter,
            SignalBus signalBus, IActionExecutor actionExecutor) {
            router = NullGuard.NotNullOrThrow(flowRouter);
            this.flowModel = NullGuard.NotNullOrThrow(flowModel);
            currentNode = NullGuard.NotNullOrThrow(startNode);
            this.signalBus = NullGuard.NotNullOrThrow(signalBus);
            this.actionExecutor = NullGuard.NotNullOrThrow(actionExecutor);

            visitedNodeIds.Clear(); // shouldn't be needed
        }

        internal static IFlowAggregateFacade create(IPlacedEntryPoint placedEntryPoint, long power,
            IFlowRouter flowRouter, SignalBus signalBus, IActionExecutor actionExecutor) {
            // sourceId ??= CorrelationId.NextString();
            var payload = new FlowSeed(power);
            var context = new FlowContext(placedEntryPoint);
            var model = new FlowModel(payload, context);
            var startNode = placedEntryPoint;

            return new FlowAggregate(model, startNode, flowRouter, signalBus, actionExecutor);
        }

        public void start() {
            _ = startAsync();
        }

        public void addPower(PowerAmount damageAmount) {
            flowModel.addPower(damageAmount);

            signalBus.Fire(new ItemPowerChangedDtoEvent(currentNode.getId(), damageAmount.getPower()));
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
                while (!ct.IsCancellationRequested && currentNode != null && flowModel != null) {
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
            IActionDescription actionSpecification = currentNode.prepareItemActionDescription();

            // IPreparedAction preparedAction = actionSpecification.ToPreparedAction(this);
            ExecuteActionCommand executeActionCommand = new ExecuteActionCommand(actionSpecification, this);

            try {
                // await _actionExecutor.ExecuteAsync(preparedAction, cancellationToken);
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

        private async Task<bool> goNextAsync(CancellationToken ct) {
            NullGuard.NotNullCheckOrThrow(currentNode, flowModel);

            var decision = router.decideNext(currentNode, visitedNodeIds);
            if (decision is null) {
                FlowCompletionDispatcher
                    .finishFlow(flowModel); // TODO: change it. Use service or something like that instead
                currentNode = null;
                return false;
            }

            currentNode = decision;
            flowModel.getFlowContext().nextStep();

            await Task.Yield();
            return true;
        }

        public void stop() {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }
}