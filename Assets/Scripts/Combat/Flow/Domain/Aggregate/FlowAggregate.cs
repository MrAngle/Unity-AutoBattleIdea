
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.Flow.Domain.Router;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using Inventory.Items.View;
using Shared.Utility;
using UnityEngine;
using Zenject;

namespace Combat.Flow.Domain.Aggregate
{
    
    /// Agregat odpowiedzialny za przeprowadzenie modelu przez kolejne kroki.
    public class FlowAggregate : IFlowAggregateFacade
    {
        private readonly IFlowRouter _router;
        private readonly FlowModel _flowModel;
        private readonly SignalBus _signalBus;
        private IPlacedItem _currentNode;

        public IReadOnlyList<long> VisitedNodeIds => _visitedNodeIds;
        private readonly List<long> _visitedNodeIds = new();
        private bool _running;
        
        private CancellationTokenSource _cts;
        
        // public event Action<FlowPowerDeltaApplied> OnPowerDeltaApplied;
        
        private FlowAggregate(FlowModel flowModel, PlacedEntryPoint startNode, IFlowRouter flowRouter, SignalBus signalBus)
        {
            _router    = NullGuard.NotNullOrThrow(flowRouter);
            _flowModel = NullGuard.NotNullOrThrow(flowModel);
            _currentNode = NullGuard.NotNullOrThrow(startNode);
            _signalBus = NullGuard.NotNullOrThrow(signalBus);

            _visitedNodeIds.Clear();
        }

        /// Inicjuje przepływ od węzła startowego.
        public static IFlowAggregateFacade Create(PlacedEntryPoint placedEntryPoint, long power, IFlowRouter flowRouter, SignalBus signalBus)
        {
            // sourceId ??= CorrelationId.NextString();
            var payload = new FlowSeed(power);
            var context = new FlowContext(placedEntryPoint);
            var model = new FlowModel(payload, context);
            var startNode = placedEntryPoint;
            
            return new FlowAggregate(model, startNode, flowRouter, signalBus);
        }
        
        public void Start() {
            _ = StartAsync(); 
            // Step();
        }

        /// Wykonuje logikę bieżącego węzła (mutuje Model).
        // public void Process()
        // {
        //     if (_currentNode == null || _flowModel == null) return;
        //
        //     _currentNode.Process(this);
        //     _visitedNodeIds.Add(_currentNode.GetId());
        // }

        // public void GoNext() {
        //     NullGuard.NotNullCheckOrThrow(_currentNode, _flowModel);
        //
        //     var decision = _router.DecideNext(_currentNode, _flowModel, _visitedNodeIds);
        //     if (decision is null)
        //     {
        //         FlowCompletionDispatcher.Finish(_flowModel);
        //         return;
        //     }
        //
        //     // (opcjonalnie) możesz logować decision.Value.EntryCell do debug
        //     _currentNode = decision;
        //     _flowModel.FlowContext.NextStep();
        //     Step();
        // }

        // public void Step()
        // {
        //     if (_currentNode == null || _flowModel == null) 
        //         throw new NotImplementedException();
        //     Process();
        //     GoNext();
        // }

        public bool IsFinished => _currentNode == null || _flowModel == null;

        public void AddPower(long power) {
            _flowModel.AddPower(power);
            
            _signalBus.Fire(new ItemPowerChangedDtoEvent(_currentNode.GetId(), power));
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
        
        private async Task ProcessAsync(CancellationToken ct)
        {
            try {
            await _currentNode.ProcessAsync(this, ct);
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
