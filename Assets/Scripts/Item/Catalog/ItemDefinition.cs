using System.Collections.Generic;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;

namespace MageFactory.Item.Catalog {
    public sealed class ItemDefinition : IItemDefinition {
        private readonly ShapeArchetype shapeArchetype;

        private ItemDefinition(ShapeArchetype shape) {
            shapeArchetype = shape;
        }

        public ShapeArchetype getShape() {
            return shapeArchetype;
        }

        // 🔽 statyczne instancje = „enum values”

        public static readonly ItemDefinition GemShard =
            new ItemDefinition(
                ShapeCatalog.Square1x1
            );

        public static readonly ItemDefinition ShieldPlate =
            new ItemDefinition(
                ShapeCatalog.Square2x2
            );

        public static readonly ItemDefinition LBracket =
            new ItemDefinition(
                ShapeCatalog.LBracket
            );

        public static readonly IReadOnlyList<ItemDefinition> All = new[] {
            GemShard,
            ShieldPlate,
            LBracket
        };
    }

    public sealed class EntryPointDefinition : IEntryPointDefinition {
        private readonly ShapeArchetype shapeArchetype;
        private readonly FlowKind flowKind;

        private EntryPointDefinition(FlowKind flowKind, ShapeArchetype shapeArchetype) {
            this.shapeArchetype = shapeArchetype;
            this.flowKind = flowKind;
        }

        public ShapeArchetype getShape() {
            return shapeArchetype;
        }

        public FlowKind getFlowKind() {
            return flowKind;
        }

        // 🔽 statyczne instancje = „enum values”
        public static readonly EntryPointDefinition Standard =
            new EntryPointDefinition(
                FlowKind.Damage,
                ShapeCatalog.Square1x1
            );

        public static readonly IReadOnlyList<IEntryPointDefinition> All = new[] {
            Standard
        };
    }
}