using System;
using System.Globalization;

namespace MageFactory.UI.Component.Inventory.ItemLayer {
    public static class GuardPowerLabelFormatter {
        private static readonly string[] Suffixes = { "", "k", "m", "b", "t" };

        public static string format(long value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return formatNonNegative(value);
        }

        public static string formatNonNegativeOrSigned(long value) {
            if (value >= 0) {
                return formatNonNegative(value);
            }

            return "-" + formatNonNegative(Math.Abs(value));
        }

        private static string formatNonNegative(long value) {
            double scaledValue = value;
            int suffixIndex = 0;

            while (scaledValue >= 999.5d && suffixIndex < Suffixes.Length - 1) {
                scaledValue /= 1000d;
                suffixIndex++;
            }

            string suffix = Suffixes[suffixIndex];
            if (suffixIndex == 0) {
                return Math.Round(scaledValue).ToString(CultureInfo.InvariantCulture);
            }

            double rounded = scaledValue < 10d
                ? Math.Round(scaledValue, 1, MidpointRounding.AwayFromZero)
                : Math.Round(scaledValue, MidpointRounding.AwayFromZero);

            string number = rounded < 10d && Math.Abs(rounded - Math.Round(rounded)) > 0.001d
                ? rounded.ToString("0.#", CultureInfo.InvariantCulture)
                : Math.Round(rounded, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);

            if (number.Length + suffix.Length > 4) {
                number = Math.Round(rounded, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
            }

            return number + suffix;
        }
    }
}