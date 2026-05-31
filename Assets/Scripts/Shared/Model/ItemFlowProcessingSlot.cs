using System;
using MageFactory.Shared.Id;
using MageFactory.Shared.Utility;

namespace MageFactory.Shared.Model {
    public sealed class ItemFlowProcessingSlot : IEquatable<ItemFlowProcessingSlot> {
        private readonly Id<ItemId> itemId;
        private readonly int localRow;
        private readonly int cellCount;

        private ItemFlowProcessingSlot(
            Id<ItemId> itemId,
            int localRow,
            int cellCount) {
            this.itemId = NullGuard.ValidIdOrThrow(itemId);
            this.localRow = localRow;

            if (cellCount <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(cellCount),
                    cellCount,
                    "Item flow processing slot must contain at least one cell.");
            }

            this.cellCount = cellCount;
        }

        public static ItemFlowProcessingSlot of(
            Id<ItemId> itemId,
            int localRow,
            int cellCount) {
            return new ItemFlowProcessingSlot(itemId, localRow, cellCount);
        }

        public Id<ItemId> getItemId() {
            return itemId;
        }

        public int getLocalRow() {
            return localRow;
        }

        public int getCellCount() {
            return cellCount;
        }

        public bool Equals(ItemFlowProcessingSlot other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return itemId.Equals(other.itemId)
                   && localRow == other.localRow
                   && cellCount == other.cellCount;
        }

        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj)
                   || obj is ItemFlowProcessingSlot other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = itemId.GetHashCode();
                hashCode = (hashCode * 397) ^ localRow;
                hashCode = (hashCode * 397) ^ cellCount;
                return hashCode;
            }
        }
    }
}