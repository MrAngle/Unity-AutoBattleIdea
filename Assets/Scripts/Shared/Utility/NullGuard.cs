using System;
using System.Linq.Expressions;

namespace Shared.Utility {
    public static class NullGuard {

        public static T NotNullOrThrow<T>(T value, string paramName = null) where T : class
        {
            if (value is null)
                throw new ArgumentNullException(
                    paramName ?? typeof(T).Name
                );

            return value;
        }
        
        public static T NotNullOrThrow<T>(T? value, string paramName = null) where T : struct
        {
            if (value is null)
                throw new ArgumentNullException(paramName ?? typeof(T).Name);
            return value.Value;
        }
        
        public static void NotNullCheckOrThrow(params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i] is null)
                    throw new ArgumentNullException($"arg{i + 1}");
        }
    }
}