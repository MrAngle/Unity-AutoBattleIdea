using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Domain {
    internal class InventoryCell {
        // TODO: make it internal
        public CellState State { get; set; }

        public InventoryCell(CellState state = CellState.Unreachable) {
            State = state;
        }

        public bool IsAvailableForPlacement => State == CellState.Empty;
    }
}