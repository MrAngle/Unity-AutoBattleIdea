
using System.Collections.Generic;
using Combat.Flow.Domain.Router;
using Shared.Utility;
using UnityEngine;

namespace Combat.Flow.Domain.Aggregate
{
    /// Agregat odpowiedzialny za przeprowadzenie modelu przez kolejne kroki.
    public class FlowAggregate
    {
        private readonly IFlowRouter _router;
        private readonly FlowModel _flowModel;

        public IReadOnlyList<long> VisitedNodeIds => _visitedNodeIds;
        private readonly List<long> _visitedNodeIds = new();
        
        private IFlowNode _currentNode;
        
        private FlowAggregate(FlowModel model, IFlowNode startNode)
        {
            _flowModel = model;
            _currentNode = startNode;
            _visitedNodeIds.Clear();
        }

        /// Inicjuje przepływ od węzła startowego.
        public static FlowAggregate Start(FlowKind kind, long power, Vector2Int startNodeCoordination, string sourceId)
        {
            sourceId ??= CorrelationId.NextString();
            var payload = new FlowSeed(power);
            var context = new FlowContext(kind, sourceId);
            var model = new FlowModel(payload, context);
            var startNode = new DummyFlowNode(startNodeCoordination, 2222);
            
            return new FlowAggregate(model, startNode);
        }
        
        public FlowModel GetModel() => _flowModel;

        /// Wykonuje logikę bieżącego węzła (mutuje Model).
        public void Process()
        {
            if (_currentNode == null || _flowModel == null) return;

            _currentNode.Process(this);
            _visitedNodeIds.Add(_currentNode.GetId());
        }

        public void GoNext()
        {
            if (_currentNode == null || _flowModel == null) return;

            var decision = _router.DecideNext(_currentNode, _flowModel, _visitedNodeIds);
            if (decision is null)
            {
                _currentNode = null; // koniec
                return;
            }

            // (opcjonalnie) możesz logować decision.Value.EntryCell do debug
            _currentNode = decision.Value.NextNode;
            _flowModel.FlowContext.NextStep();
        }

        public void Step()
        {
            if (_currentNode == null || _flowModel == null) 
                return;
            Process();
            GoNext();
        }

        public bool IsFinished => _currentNode == null || _flowModel == null;
    }
}
