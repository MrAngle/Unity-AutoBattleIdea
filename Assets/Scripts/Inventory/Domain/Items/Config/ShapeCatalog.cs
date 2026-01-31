// using System.Collections.Generic;
// using Inventory.Items.Domain;
// using Shared.Utility;
//
// namespace Inventory.Items.Config {
//     public sealed class ShapeArchetypeId : ConstantId<ShapeArchetypeId> {
//         public static readonly ShapeArchetypeId SQUARE_1X1 = Define(1);
//         public static readonly ShapeArchetypeId SQUARE_2X2 = Define(2);
//         public static readonly ShapeArchetypeId L_BRACKET = Define(3);
//     }
//
//     /// <summary>
//     ///     “Twardo” skonfigurowane przykładowe przedmioty (bez ScriptableObjectów).
//     ///     W dowolnym momencie możesz to zastąpić odczytem z SO/JSON, a interfejs pozostanie ten sam (ItemData).
//     /// </summary>
//     public static class ShapeCatalog {
//         // 1x1
//         public static readonly ShapeArchetype Square1x1 = new(
//             ShapeArchetypeId.SQUARE_1X1,
//             "Gem Shard",
//             ItemShape.SingleCell(),
//             "Icons/GemShard"
//         );
//
//         // 2x2
//         public static readonly ShapeArchetype Square2x2 = new(
//             ShapeArchetypeId.SQUARE_2X2,
//             "Shield Plate (2x2)",
//             ItemShape.Square2x2(),
//             "Icons/ShieldPlate"
//         );
//
//         // L z 4 pól
//         public static readonly ShapeArchetype LBracket = new(
//             ShapeArchetypeId.L_BRACKET,
//             // itemDataId: ItemDataId.,
//             "L-Bracket",
//             ItemShape.LShape(),
//             "Icons/LBracket"
//         );
//
//         public static readonly IReadOnlyList<ShapeArchetype> All = new[] {
//             Square1x1,
//             Square2x2,
//             LBracket
//         };
//     }
// }

