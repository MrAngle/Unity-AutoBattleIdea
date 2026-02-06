// using System;
// using System.Collections.Generic;
// using System.Linq;
// using MageFactory.ActionEffect;
// using MageFactory.ActionExecutor.Api;
// using MageFactory.Inventory.Api;
//
// namespace MageFactory.ActionExecutor.Domain {
//     public sealed class ActionEffectResolver {
//         private readonly Dictionary<Type, IEffectMapper> mappers;
//
//         public ActionEffectResolver(IEnumerable<IEffectMapper> mappers) {
//             this.mappers = mappers.ToDictionary(m => m.DescriptorType);
//         }
//
//         public IEffect Resolve(IEffectsDescriptor descriptor) {
//             var type = descriptor.GetType();
//
//             if (!mappers.TryGetValue(type, out var mapper)) {
//                 throw new InvalidOperationException(
//                     $"No mapper registered for descriptor type {type.FullName}");
//             }
//
//             return mapper.Map(descriptor);
//         }
//     }
// }

