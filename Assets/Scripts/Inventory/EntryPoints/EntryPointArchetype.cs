using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Combat.Flow.Domain;
using Combat.Flow.Domain.Aggregate;
using Inventory.Items.Domain;
using Inventory.Position;
using Registry;
using Shared.Utility;
using UnityEngine;
using Random = System.Random;

namespace Inventory.EntryPoints {
    public sealed class GridEntryPointId : StrongId<GridEntryPointId> {
    }
    
    public sealed class EntryPointArchetype : IPlaceableItem {

        private readonly IEntryPointFactory _entryPointFactory; // separate in future
        
        private readonly float _turnInterval;
        private readonly FlowKind _kind;
        private readonly ShapeArchetype _shapeArchetype;

        private bool _battleRunning;
        private CancellationTokenSource _cts;

        private EntryPointArchetype(FlowKind kind, ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) {
            _entryPointFactory = NullGuard.NotNullOrThrow(entryPointFactory);
            
            _kind = kind;
            _shapeArchetype = NullGuard.NotNullOrThrow(shapeArchetype);

            _turnInterval = Mathf.Max(0.01f, 2.5f);
        }

        public static EntryPointArchetype Create(FlowKind kind, ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) {
            return new EntryPointArchetype(kind, shapeArchetype, entryPointFactory);
        }

        public IPlacedItem ToPlacedItem(IGridInspector gridInspector, Vector2Int origin) {
            return _entryPointFactory.CreatePlacedEntryPoint(this, origin, gridInspector);
        }

        public ShapeArchetype GetShape() {
            return _shapeArchetype;
        }

        public FlowKind GetFlowKind() {
            return _kind;
        }

        public float GetTurnInterval() {
            return _turnInterval;
        }

        public override string ToString() {
            return $"({_kind})";
        }
    }
}