
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.ActionExecutor;
using Combat.Flow.Domain.Router;
using Combat.Flow.Domain.Shared;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using Inventory.Items.View;
using Shared.Utility;
using UnityEngine;
using Zenject;

namespace Combat.Flow.Domain.Aggregate
{
    public interface IFlowContext {
        void AddPower(DamageAmount damageAmount);
        // void AddPower(DamageToReceive damageToDeal);
    }

    public class FlowAggregate : IFlowAggregateFacade, IFlowContext
    {
        private readonly IFlowRouter _router;
        private readonly FlowModel _flowModel;
        private readonly SignalBus _signalBus;
        private readonly IActionExecutor _actionExecutor;
        private IPlacedItem _currentNode;

        public IReadOnlyList<long> VisitedNodeIds => _visitedNodeIds;
        private readonly List<long> _visitedNodeIds = new();
        private bool _running;
        
        private CancellationTokenSource _cts;
        
        // public event Action<FlowPowerDeltaApplied> OnPowerDeltaApplied;
        
        private FlowAggregate(FlowModel flowModel, PlacedEntryPoint startNode, IFlowRouter flowRouter, SignalBus signalBus, IActionExecutor actionExecutor)
        {
            _router    = NullGuard.NotNullOrThrow(flowRouter);
            _flowModel = NullGuard.NotNullOrThrow(flowModel);
            _currentNode = NullGuard.NotNullOrThrow(startNode);
            _signalBus = NullGuard.NotNullOrThrow(signalBus);
            _actionExecutor = NullGuard.NotNullOrThrow(actionExecutor);
            
            _visitedNodeIds.Clear(); // shouldn't be needed
        }

        public static IFlowAggregateFacade Create(PlacedEntryPoint placedEntryPoint, long power, IFlowRouter flowRouter, SignalBus signalBus, IActionExecutor _actionExecutor)
        {
            // sourceId ??= CorrelationId.NextString();
            var payload = new FlowSeed(power);
            var context = new FlowContext(placedEntryPoint);
            var model = new FlowModel(payload, context);
            var startNode = placedEntryPoint;
            
            return new FlowAggregate(model, startNode, flowRouter, signalBus, _actionExecutor);
        }
        
        public void Start() {
            _ = StartAsync(); 
        }

        public bool IsFinished => _currentNode == null || _flowModel == null;

        public void AddPower(DamageAmount damageAmount) {
            _flowModel.AddPower(damageAmount);
            
            _signalBus.Fire(new ItemPowerChangedDtoEvent(_currentNode.GetId(), damageAmount.GetPower()));
        }
                
        public Task StartAsync()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            return StepLoopAsync(_cts.Token);
        }
        
        private async Task StepLoopAsync(CancellationToken ct)
        {
            if (_running) return;
            _running = true;
            try
            {
                while (!ct.IsCancellationRequested && _currentNode != null && _flowModel != null)
                {
                    await ProcessAsync(ct);
                    if (ct.IsCancellationRequested) break;
                    if (!await GoNextAsync(ct)) break;
                }
            }
            finally { _running = false; }
        }
        
        private async Task ProcessAsync(CancellationToken cancellationToken) {
            IActionSpecification actionSpecification = _currentNode.GetAction();

            IPreparedAction preparedAction = actionSpecification.ToPreparedAction(this);
            
            try {
                await _actionExecutor.ExecuteAsync(preparedAction, cancellationToken);
            } catch (OperationCanceledException) {
                throw; // for now
            } catch (Exception  ex) {
                Debug.LogException(ex);
                throw; // for now
            }

            _visitedNodeIds.Add(_currentNode.GetId());
        }

        private async Task<bool> GoNextAsync(CancellationToken ct)
        {
            NullGuard.NotNullCheckOrThrow(_currentNode, _flowModel);

            var decision = _router.DecideNext(_currentNode, _flowModel, _visitedNodeIds);
            if (decision is null)
            {
                FlowCompletionDispatcher.Finish(_flowModel);
                _currentNode = null;
                return false;
            }

            _currentNode = decision;
            _flowModel.FlowContext.NextStep();

            await Task.Yield();
            return true;
        }
        
        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }


}
