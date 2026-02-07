namespace MageFactory.Shared.Model {
    public enum CellState {
        Unreachable, // nieosiągalne
        Empty, // dozwolone i puste

        Occupied // dozwolone ale zajęte
        // (możesz dodać: Locked / Reserved)
    }
}