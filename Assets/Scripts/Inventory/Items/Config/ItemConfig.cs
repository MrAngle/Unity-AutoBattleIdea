using System.Collections.Generic;
using UI.Inventory.Items.Domain;

namespace Inventory.Items.Config
{
    /// <summary>
    /// “Twardo” skonfigurowane przykładowe przedmioty (bez ScriptableObjectów).
    /// W dowolnym momencie możesz to zastąpić odczytem z SO/JSON, a interfejs pozostanie ten sam (ItemData).
    /// </summary>
    public static class ItemConfig
    {
        // 1x1
        public static readonly ItemData GemShard = new(
            id: "gem_shard",
            displayName: "Gem Shard",
            shape: ItemShape.SingleCell(),
            iconId: "Icons/GemShard"
        );

        // 2x2
        public static readonly ItemData ShieldPlate2x2 = new(
            id: "shield_plate_2x2",
            displayName: "Shield Plate (2x2)",
            shape: ItemShape.Square2x2(),
            iconId: "Icons/ShieldPlate"
        );

        // L z 4 pól
        public static readonly ItemData LBracket = new(
            id: "l_bracket",
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