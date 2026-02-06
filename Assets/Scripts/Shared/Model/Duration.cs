namespace MageFactory.Shared.Model {
    public record Duration(float duration) {
        public float getValue() {
            return duration;
        }
    }
}