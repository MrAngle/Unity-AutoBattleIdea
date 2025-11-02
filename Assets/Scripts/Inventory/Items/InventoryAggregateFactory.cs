using System.Collections.Generic;
using Inventory.EntryPoints;
using Inventory.Items.Domain;
using Zenject;
using UnityEngine;
using Combat.Flow.Domain.Aggregate;
using Inventory.Slots.Domain;

namespace Inventory.Items
{
    public interface IInventoryAggregateFactory
    {
        InventoryAggregate Create();
    }
    
    
    public class InventoryAggregateFactory : IInventoryAggregateFactory
    {
        private readonly SignalBus _signalBus;
        private readonly IEntryPointFactory _entryPointFactory;

        [Inject]
        public InventoryAggregateFactory(
            SignalBus signalBus,
            IEntryPointFactory entryPointFactory)
        {
            _signalBus = signalBus;
            _entryPointFactory = entryPointFactory;
        }


        public InventoryAggregate Create() {
            InventoryAggregate inventoryAggregate = InventoryAggregate.Create(_signalBus, _entryPointFactory);

            inventoryAggregate.Place(GridEntryPoint.Create(FlowKind.Damage), new Vector2Int(0, 0)); // for now
            
            return inventoryAggregate;
        }
    }
}