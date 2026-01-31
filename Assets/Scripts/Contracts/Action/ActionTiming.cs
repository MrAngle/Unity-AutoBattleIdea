namespace Contracts.Actionexe {
    public readonly struct ActionTiming {
        private readonly float _durationSeconds;

        public ActionTiming(float durationSeconds) {
            _durationSeconds = durationSeconds;
        }

        public float DurationSeconds() {
            return _durationSeconds;
        }
    }
}