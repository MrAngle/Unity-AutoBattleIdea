
using System;
using System.Collections.Generic;
using Combat.Flow.Domain.Router;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using Shared.Utility;
using UnityEngine;

namespace Combat.Flow.Domain.Aggregate
{
    /// Agregat odpowiedzialny za przeprowadzenie modelu przez kolejne kroki.
    public class FlowAggregate : IFlowAggregateFacade
    {
        private readonly IFlowRouter _router;
        private readonly FlowModel _flowModel;
        private IPlacedItem _currentNode;

        public IReadOnlyList<long> VisitedNodeIds => _visitedNodeIds;
        private readonly List<long> _visitedNodeIds = new();
        
        private FlowAggregate(FlowModel flowModel, PlacedEntryPoint startNode, IFlowRouter flowRouter)
        {
            _router    = NullGuard.NotNullOrThrow(flowRouter);
            _flowModel = NullGuard.NotNullOrThrow(flowModel);
            _currentNode = NullGuard.NotNullOrThrow(startNode);

            _visitedNodeIds.Clear();
        }

        /// Inicjuje przepływ od węzła startowego.
        public static IFlowAggregateFacade Create(PlacedEntryPoint placedEntryPoint, long power, IFlowRouter flowRouter)
        {
            // sourceId ??= CorrelationId.NextString();
            var payload = new FlowSeed(power);
            var context = new FlowContext(placedEntryPoint);
            var model = new FlowModel(payload, context);
            var startNode = placedEntryPoint;
            
            return new FlowAggregate(model, startNode, flowRouter);
        }
        
        public FlowModel GetModel() => _flowModel;
        
        public void Start() {
            Step();
        }

        /// Wykonuje logikę bieżącego węzła (mutuje Model).
        public void Process()
        {
            if (_currentNode == null || _flowModel == null) return;

            _currentNode.Process(this);
            _visitedNodeIds.Add(_currentNode.GetId());
        }

        public void GoNext() {
            NullGuard.NotNullCheckOrThrow(_currentNode, _flowModel);

            var decision = _router.DecideNext(_currentNode, _flowModel, _visitedNodeIds);
            if (decision is null)
            {
                _currentNode = null; // koniec
                throw new NotImplementedException();
                return;
            }

            // (opcjonalnie) możesz logować decision.Value.EntryCell do debug
            _currentNode = decision;
            _flowModel.FlowContext.NextStep();
        }

        public void Step()
        {
            if (_currentNode == null || _flowModel == null) 
                throw new NotImplementedException();
            Process();
            GoNext();
        }

        public bool IsFinished => _currentNode == null || _flowModel == null;

    }
}
