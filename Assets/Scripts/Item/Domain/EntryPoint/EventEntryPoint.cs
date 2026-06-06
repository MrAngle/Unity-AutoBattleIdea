using MageFactory.ActionEffect;
using MageFactory.Inventory.Contract;

namespace MageFactory.Item.Domain.EntryPoint {
    internal class EventEntryPoint : EntryPointArchetype {
        internal EventEntryPoint(IEntryPointDefinition itemDefinition,
                                 IEntryPointFactory entryPointFactory) :
            base(itemDefinition, entryPointFactory) {
        }

        internal static IEntryPointArchetype create(IEntryPointDefinition entryPointDefinition,
                                                    IEntryPointFactory entryPointFactory) {
            return new EventEntryPoint(entryPointDefinition, entryPointFactory);
        }
    }
}