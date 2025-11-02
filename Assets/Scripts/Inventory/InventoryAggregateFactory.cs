using Combat.Flow.Domain.Aggregate;
using Inventory.EntryPoints;
using Inventory.Items.Config;
using UnityEngine;
using Zenject;

namespace Inventory {
    public interface IInventoryAggregateFactory {
        InventoryAggregate Create();
    }


    public class InventoryAggregateFactory : IInventoryAggregateFactory {
        private readonly IEntryPointFactory _entryPointFactory;
        private readonly SignalBus _signalBus;

        [Inject]
        public InventoryAggregateFactory(
            SignalBus signalBus,
            IEntryPointFactory entryPointFactory) {
            _signalBus = signalBus;
            _entryPointFactory = entryPointFactory;
        }


        public InventoryAggregate Create() {
            var inventoryAggregate = InventoryAggregate.Create(_signalBus, _entryPointFactory);

            EntryPointArchetype entryPointArchetype = EntryPointArchetype.Create(FlowKind.Damage, ShapeCatalog.Square1x1, _entryPointFactory);

            inventoryAggregate.Place(entryPointArchetype, new Vector2Int(0, 0)); // for now

            return inventoryAggregate;
        }
    }
}