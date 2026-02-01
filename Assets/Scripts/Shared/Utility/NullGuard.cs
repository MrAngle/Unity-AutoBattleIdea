using System;

namespace MageFactory.Shared.Utility {
    public static class NullGuard {
        public static T NotNullOrThrow<T>(T value, string paramName = null) where T : class {
            if (value is null)
                throw new ArgumentNullException(
                    paramName ?? typeof(T).Name
                );

            return value;
        }

        public static T NotNullOrThrow<T>(T? value, string paramName = null) where T : struct {
            if (value is null)
                throw new ArgumentNullException(paramName ?? typeof(T).Name);
            return value.Value;
        }

        public static void NotNullCheckOrThrow(params object[] values) {
            for (int i = 0; i < values.Length; i++)
                if (values[i] is null)
                    throw new ArgumentNullException($"arg{i + 1}");
        }

        public static TEnum enumDefinedOrThrow<TEnum>(
            TEnum value,
            string paramName = null
        )
            where TEnum : struct, Enum {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentOutOfRangeException(
                    paramName ?? typeof(TEnum).Name,
                    value,
                    $"Invalid value for enum {typeof(TEnum).Name}"
                );

            return value;
        }
    }
}