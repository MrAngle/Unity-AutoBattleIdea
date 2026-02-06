namespace MageFactory.Shared.Model {
    public record Duration(float duration) {
        public float durationInSeconds() {
            return duration;
        }

        public float getValue() {
            return duration;
        }
    }

    // public readonly struct Duration() {
    //     private readonly float _durationSeconds;
    //
    //     public ActionTiming(float durationSeconds) {
    //         _durationSeconds = durationSeconds;
    //     }
    //
    //     public float DurationSeconds() {
    //         return _durationSeconds;
    //     }
    // }
}