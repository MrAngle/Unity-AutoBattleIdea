using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Domain {
    internal class InventoryCell {
        public CellState State { get; set; }

        public InventoryCell(CellState state = CellState.Unreachable) {
            State = state;
        }

        public bool IsAvailableForPlacement => State == CellState.Empty;
    }
}