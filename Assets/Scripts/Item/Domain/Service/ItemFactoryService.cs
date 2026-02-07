using System.Runtime.CompilerServices;
using MageFactory.Flow.Api;
using MageFactory.Item.Api;
using MageFactory.Item.Api.Dto;
using MageFactory.Item.Controller.Api;
using MageFactory.Item.Domain.EntryPoint;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using MageFactory.Shared.Utility;
using UnityEngine;
using Zenject;

[assembly: InternalsVisibleTo("MageFactory.InjectConfiguration")]

namespace MageFactory.Item.Domain.Service {
    internal class ItemFactoryService : IEntryPointFactory, IItemFactory {
        private readonly IFlowFactory _flowFactory;

        [Inject]
        internal ItemFactoryService(IFlowFactory flowFactory) {
            _flowFactory = NullGuard.NotNullOrThrow(flowFactory);
        }

        public IPlacedEntryPoint createPlacedEntryPoint(IEntryPointArchetype archetype, Vector2Int position,
            IGridInspector gridInspector) {
            var placedEntryPoint = PlacedEntryPoint.create(archetype, position, gridInspector, _flowFactory);

            return placedEntryPoint;
        }

        public IEntryPointArchetype createArchetypeEntryPoint(FlowKind kind, ShapeArchetype shapeArchetype) {
            return new TickEntryPoint(kind, shapeArchetype, this);
        }

        public IPlaceableItem createPlacableItem(CreatePlaceableItemCommand createPlaceableItemCommand) {
            return ItemArchetype.create(createPlaceableItemCommand);
        }
    }
}