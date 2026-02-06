// using System;
// using MageFactory.Inventory.Api;
//
// namespace MageFactory.ActionExecutor.Domain {
//     public interface IEffectMapper {
//         Type DescriptorType { get; }
//         IActionEffect Map(IEffectsDescriptor descriptor);
//     }
//
//     public abstract class EffectMapper<TDescriptor> : IEffectMapper
//         where TDescriptor : IEffectsDescriptor {
//         
//         public Type DescriptorType => typeof(TDescriptor);
//
//         public IActionEffect Map(IEffectsDescriptor descriptor) {
//             return MapTyped((TDescriptor)descriptor);
//         }
//
//         protected abstract IActionEffect MapTyped(TDescriptor descriptor);
//     }
//
// }

