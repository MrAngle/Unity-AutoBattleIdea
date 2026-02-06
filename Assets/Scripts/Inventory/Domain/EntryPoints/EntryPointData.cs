using System.Threading;
using MageFactory.Inventory.Api;
using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Domain {
    public class EntryPointData {
        private readonly IEntryPointFactory _entryPointFactory; // separate in future
        private readonly FlowKind _kind;
        private readonly ShapeArchetype _shapeArchetype;

        private readonly float _turnInterval;

        private bool _battleRunning;
        private CancellationTokenSource _cts;
    }
}