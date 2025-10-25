using System;
using System.Threading;

namespace Shared.Utility {
    public static class CorrelationId
    {
        private static long _counter;

        public static long Next() => Interlocked.Increment(ref _counter);

        /// Tekst dziesiętny (np. "123456").
        public static string NextString()
        {
            long id = Next();
            Span<char> buf = stackalloc char[20];
            id.TryFormat(buf, out int written);
            return new string(buf[..written]);
        }
    }
}

