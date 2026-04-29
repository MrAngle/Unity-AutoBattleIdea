using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.Shared.Id;

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

        public static Id<T> ValidIdOrThrow<T>(Id<T> id, string paramName = null) {
            if (id.Value <= 0) {
                throw new ArgumentOutOfRangeException(
                    paramName ?? typeof(T).Name,
                    id,
                    "Id must be greater than 0."
                );
            }

            return id;
        }

        public static void NotNullCheckOrThrow(params object[] values) {
            for (int i = 0; i < values.Length; i++)
                if (values[i] is null)
                    throw new ArgumentNullException($"arg{i + 1}");
        }

        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> source, string paramName = null) {
            if (source is null) {
                throw new ArgumentException("Collection null or empty.", paramName ?? nameof(source));
            }

            if (source.IsNullOrEmpty()) {
                throw new ArgumentException("Collection cannot be empty.", paramName ?? nameof(source));
            }
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

        private static bool IsNullOrEmpty<T>(this IEnumerable<T> source) {
            return source == null || !source.Any();
        }
    }
}