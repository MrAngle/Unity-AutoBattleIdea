using System.Threading;
using Combat.ActionExecutor;
using Combat.Flow.Domain.Aggregate;
using Combat.Flow.Domain.Shared;
using Inventory.Items.Domain;
using Shared.Utility;
using UI.Combat.Action;
using UnityEngine;

namespace Inventory.EntryPoints {
    
    public sealed class EventEntryPoint : EntryPointArchetype {

        private EventEntryPoint(ShapeArchetype shapeArchetype, IEntryPointFactory entryPointFactory) : base(FlowKind.Defense, shapeArchetype, entryPointFactory) {

        }

        protected override ActionCommandDescriptor PrepareActionCommandDescriptor(IEntryPointContext entryPointContext) {
            return new ActionCommandDescriptor(
                new AddPower(new DamageToReceive(3))
            );
        } 

    }
}