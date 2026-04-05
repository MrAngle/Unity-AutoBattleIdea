using System.Collections.Generic;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.ActionEffect.PredefinedOperations {
    public sealed class PushItem : IOperation {
        private readonly GridDirection pushDirection;
        private readonly IReadOnlyList<GridDirection> relatedItemsRelatedToPush;

        public PushItem(GridDirection pushDirection, IReadOnlyList<GridDirection> relatedItemsRelatedToPush) {
            this.pushDirection = NullGuard.enumDefinedOrThrow(pushDirection);
            this.relatedItemsRelatedToPush = NullGuard.NotNullOrThrow(relatedItemsRelatedToPush);
        }

        public void apply(IActionCapabilities actionCapabilities) {
            actionCapabilities.command().pushRightAdjacentItemRight();
        }
    }
}