using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.ActionExecutor;
using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

namespace Combat.Flow.Domain.Aggregate {
    public class FlowAggregate : IFlowAggregateFacade, IFlowContext {
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

        public void AddPower(DamageAmount damageAmount) {
            _flowModel.AddPower(damageAmount);

            _signalBus.Fire(new ItemPowerChangedDtoEvent(_currentNode.GetId(), damageAmount.GetPower()));
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
            var actionSpecification = _currentNode.GetAction();

            var preparedAction = actionSpecification.ToPreparedAction(this);

            try {
                await _actionExecutor.ExecuteAsync(preparedAction, cancellationToken);
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
                FlowCompletionDispatcher.Finish(_flowModel);
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