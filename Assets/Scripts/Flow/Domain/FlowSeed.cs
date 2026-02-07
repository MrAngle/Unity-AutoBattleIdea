namespace MageFactory.Flow.Domain {
    internal class FlowSeed {
        private readonly long _initPower;

        internal FlowSeed(long power) {
            _initPower = power;
        }

        internal long power() {
            return _initPower;
        }
    }
}