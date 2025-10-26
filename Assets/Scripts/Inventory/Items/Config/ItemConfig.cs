using System.Collections.Generic;
using Inventory.Items.Domain;
using Shared.Utility;

namespace Inventory.Items.Config
{
    public sealed class ItemDataId : ConstantId<ItemDataId>
    {
        public static readonly ItemDataId GEM_SHARD         = Define(1);
        public static readonly ItemDataId SHIELD_PLATE_2X2  = Define(2);
        public static readonly ItemDataId L_BRACKET         = Define(3);
    }
    
    /// <summary>
    /// “Twardo” skonfigurowane przykładowe przedmioty (bez ScriptableObjectów).
    /// W dowolnym momencie możesz to zastąpić odczytem z SO/JSON, a interfejs pozostanie ten sam (ItemData).
    /// </summary>
    public static class ItemConfig {
        
        // 1x1
        public static readonly ItemData GemShard = new(
            itemDataId: ItemDataId.GEM_SHARD,
            displayName: "Gem Shard",
            shape: ItemShape.SingleCell(),
            iconId: "Icons/GemShard"
        );

        // 2x2
        public static readonly ItemData ShieldPlate2x2 = new(
            itemDataId: ItemDataId.SHIELD_PLATE_2X2,
            displayName: "Shield Plate (2x2)",
            shape: ItemShape.Square2x2(),
            iconId: "Icons/ShieldPlate"
        );

        // L z 4 pól
        public static readonly ItemData LBracket = new(
            itemDataId: ItemDataId.L_BRACKET,
            // itemDataId: ItemDataId.,
            displayName: "L-Bracket",
            shape: ItemShape.LShape(),
            iconId: "Icons/LBracket"
        );

        public static readonly IReadOnlyList<ItemData> All = new[]
        {
            GemShard,
            ShieldPlate2x2,
            LBracket
        };
    }
}