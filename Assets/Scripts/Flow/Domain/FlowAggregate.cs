using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Character.Contract.Event;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
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
        private IFlowItem currentNode;
        private bool isRunning;

        private FlowAggregate(FlowModel flowModel, IFlowItem startNode, IFlowRouter flowRouter,
                              SignalBus signalBus, IActionExecutor actionExecutor) {
            router = NullGuard.NotNullOrThrow(flowRouter);
            this.flowModel = NullGuard.NotNullOrThrow(flowModel);
            currentNode = NullGuard.NotNullOrThrow(startNode);
            this.signalBus = NullGuard.NotNullOrThrow(signalBus);
            this.actionExecutor = NullGuard.NotNullOrThrow(actionExecutor);

            visitedNodeIds.Clear(); // shouldn't be needed
        }

        internal static IFlowAggregateFacade create(IFlowItem startItem,
                                                    IFlowRouter flowRouter,
                                                    SignalBus signalBus,
                                                    IActionExecutor actionExecutor,
                                                    IFlowConsumer flowConsumer,
                                                    IFlowOwner flowOwner) {
            var context = new FlowContext(startItem, flowConsumer, flowOwner);
            var model = new FlowModel(context);
            var startNode = startItem;

            return new FlowAggregate(model, startNode, flowRouter, signalBus, actionExecutor);
        }

        public void addPower(PowerAmount damageAmount) {
            flowModel.addPower(damageAmount);

            signalBus.Fire(new ItemPowerChangedDtoEvent(currentNode.getId(), damageAmount.getPower()));
        }

        public void pushRightAdjacentItemRight() {
            // find right adjacent item
            // capabilities.query().getInventoryAggregate().getAdjacentItems()

            // move right
            // foundItem.moveRight();
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
            var actionSpecification = currentNode.prepareItemActionDescription();
            var executeActionCommand = new ExecuteActionCommand(actionSpecification, this);

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
            NullGuard.NotNullCheckOrThrow(currentNode, flowModel);

            var decision = router.decideNext(currentNode, visitedNodeIds);
            if (decision is null) {
                finishFlow();
                // FlowCompletionDispatcher
                //     .finishFlow(flowModel); // TODO: change it. Use service or something like that instead
                currentNode = null;
                return false;
            }

            currentNode = decision;
            flowModel.getFlowContext().nextStep();

            await Task.Yield();
            return true;
        }

        public void finishFlow() {
            FlowContext flowContext = flowModel.getFlowContext();
            IFlowConsumer flowConsumer = flowContext.getFlowConsumer();

            ProcessFlowCommand flowCommand =
                new(flowContext.getFlowOwner(), flowModel.getFlowPayload().getDamageToDeal());
            flowConsumer.consumeFlow(flowCommand);
        }

        public void stop() {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }
}