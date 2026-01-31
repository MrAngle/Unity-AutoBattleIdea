using System.Threading;
using Contracts.Actionexe;
using Contracts.Flow;
using Contracts.Inventory;
using Contracts.Items;
using Shared.Utility;
using UnityEngine;

namespace Inventory.EntryPoints {
    // public sealed class GridEntryPointId : StrongId<GridEntryPointId> {
    // }

    public interface IEntryPointContext {
        // IEntryPointOwner Owner { get; }
        IGridInspector Grid { get; }

        IFlowRouter FlowRouter { get; }
        // ewentualnie RNG, info o drużynach, itd.
    }

    public abstract class EntryPointArchetype : IPlaceableItem {
        private readonly IEntryPointFactory _entryPointFactory; // separate in future
        private readonly FlowKind _kind;
        private readonly ShapeArchetype _shapeArchetype;

        private readonly float _turnInterval;

        private bool _battleRunning;
        private CancellationTokenSource _cts;

        protected EntryPointArchetype(FlowKind kind, ShapeArchetype shapeArchetype,
            IEntryPointFactory entryPointFactory) {
            _entryPointFactory = NullGuard.NotNullOrThrow(entryPointFactory);

            _kind = kind;
            _shapeArchetype = NullGuard.NotNullOrThrow(shapeArchetype);

            _turnInterval = Mathf.Max(0.01f, 2.5f);
        }

        // public static EntryPointArchetype Create(FlowKind kind, ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) {
        //     return new EntryPointArchetype(kind, shapeArchetype, entryPointFactory);
        // }

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

        public IActionSpecification GetAction(IEntryPointContext entryPointContext) {
            var actionSpecification = new ActionSpecification(
                PrepareActionTiming(),
                PrepareActionCommandDescriptor(entryPointContext));

            return actionSpecification;
        }

        private ActionTiming PrepareActionTiming() {
            return new ActionTiming(2f); // for now
        }

        protected abstract ActionCommandDescriptor PrepareActionCommandDescriptor(IEntryPointContext entryPointContext);
        // {
        //     // return new ActionCommandDescriptor(
        //     //     new AddPower(new DamageToDeal(3))
        //     // );
        // } 

        public override string ToString() {
            return $"({_kind})";
        }
    }
}