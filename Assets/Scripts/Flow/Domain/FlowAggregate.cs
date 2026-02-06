using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionEffect;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Flow.Api;
using MageFactory.Flow.Domain.Service;
using MageFactory.FlowRouting;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace MageFactory.Flow.Domain {
    internal class FlowAggregate : IEffectContext, IFlowAggregateFacade {
        private readonly IActionExecutor _actionExecutor;
        private readonly FlowModel _flowModel;
        private readonly IFlowRouter _router;
        private readonly SignalBus _signalBus;
        private readonly List<long> _visitedNodeIds = new();

        private CancellationTokenSource _cts;
        private IPlacedItem _currentNode;
        private bool _running;

        // public event Action<FlowPowerDeltaApplied> OnPowerDeltaApplied;

        private FlowAggregate(FlowModel flowModel, IPlacedEntryPoint startNode, IFlowRouter flowRouter,
            SignalBus signalBus, IActionExecutor actionExecutor) {
            _router = NullGuard.NotNullOrThrow(flowRouter);
            _flowModel = NullGuard.NotNullOrThrow(flowModel);
            _currentNode = NullGuard.NotNullOrThrow(startNode);
            _signalBus = NullGuard.NotNullOrThrow(signalBus);
            _actionExecutor = NullGuard.NotNullOrThrow(actionExecutor);

            _visitedNodeIds.Clear(); // shouldn't be needed
        }

        public IReadOnlyList<long> VisitedNodeIds => _visitedNodeIds;

        public bool IsFinished => _currentNode == null || _flowModel == null;

        public void Start() {
            _ = StartAsync();
        }

        public void addPower(PowerAmount damageAmount) {
            _flowModel.AddPower(damageAmount);

            _signalBus.Fire(new ItemPowerChangedDtoEvent(_currentNode.GetId(), damageAmount.getPower()));
        }

        public static IFlowAggregateFacade Create(IPlacedEntryPoint placedEntryPoint, long power,
            IFlowRouter flowRouter, SignalBus signalBus, IActionExecutor _actionExecutor) {
            // sourceId ??= CorrelationId.NextString();
            var payload = new FlowSeed(power);
            var context = new FlowContext(placedEntryPoint);
            var model = new FlowModel(payload, context);
            var startNode = placedEntryPoint;

            return new FlowAggregate(model, startNode, flowRouter, signalBus, _actionExecutor);
        }

        public Task StartAsync() {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            return StepLoopAsync(_cts.Token);
        }

        private async Task StepLoopAsync(CancellationToken ct) {
            if (_running) return;
            _running = true;
            try {
                while (!ct.IsCancellationRequested && _currentNode != null && _flowModel != null) {
                    await ProcessAsync(ct);
                    if (ct.IsCancellationRequested) break;
                    if (!await GoNextAsync(ct)) break;
                }
            }
            finally {
                _running = false;
            }
        }

        private async Task ProcessAsync(CancellationToken cancellationToken) {
            IItemActionDescription actionSpecification = _currentNode.prepareItemActionDescription();

            // IPreparedAction preparedAction = actionSpecification.ToPreparedAction(this);
            ExecuteActionCommand executeActionCommand = new ExecuteActionCommand(actionSpecification, this);

            try {
                // await _actionExecutor.ExecuteAsync(preparedAction, cancellationToken);
                await _actionExecutor.executeAsync(executeActionCommand);
            }
            catch (OperationCanceledException) {
                throw; // for now
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                throw; // for now
            }

            _visitedNodeIds.Add(_currentNode.GetId());
        }

        private async Task<bool> GoNextAsync(CancellationToken ct) {
            NullGuard.NotNullCheckOrThrow(_currentNode, _flowModel);

            var decision = _router.DecideNext(_currentNode, _visitedNodeIds);
            if (decision is null) {
                FlowCompletionDispatcher
                    .finishFlow(_flowModel); // TODO: change it. Use service or something like that instead
                _currentNode = null;
                return false;
            }

            _currentNode = decision;
            _flowModel.FlowContext.NextStep();

            await Task.Yield();
            return true;
        }

        public void Stop() {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}