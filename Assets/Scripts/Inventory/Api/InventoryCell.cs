using MageFactory.Shared.Model;

namespace MageFactory.Inventory.Api {
    // public enum CellState {
    //     Unreachable, // nieosiągalne
    //     Empty, // dozwolone i puste
    //
    //     Occupied // dozwolone ale zajęte
    //     // (możesz dodać: Locked / Reserved)
    // }

    public class InventoryCell {
        // TODO: make it internal
        public CellState State { get; set; }

        public InventoryCell(CellState state = CellState.Unreachable) {
            State = state;
        }

        public bool IsAvailableForPlacement => State == CellState.Empty;
    }
}