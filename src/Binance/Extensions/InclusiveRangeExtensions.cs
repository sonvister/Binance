using System;

namespace Binance
{
    public static class InclusiveRangeExtensions
    {
        /// <summary>
        /// Verify a value is within range and a multiple of the increment.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValid(this InclusiveRange range, decimal value)
        {
            Throw.IfNull(range, nameof(range));

            return value >= range.Minimum && value <= range.Maximum && (value - range.Minimum) % range.Increment == 0;
        }

        /// <summary>
        /// Throw an exception if value is not within range or an increment multiple.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        public static void Validate(this InclusiveRange range, decimal value)
        {
            Throw.IfNull(range, nameof(range));

            if (value < range.Minimum)
                throw new ArgumentOutOfRangeException(nameof(value), $"Value ({value}) must be greater than or equal to minimum ({range.Minimum}).");

            if (value > range.Maximum)
                throw new ArgumentOutOfRangeException(nameof(value), $"Value ({value}) must be less than or equal to maximum ({range.Maximum}).");

            if ((value - range.Minimum) % range.Increment > 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"Value ({value}) must be a multiple of the increment ({range.Increment}).");
        }

        /// <summary>
        /// Get the nearest valid value within range coercing remainders UP to the next increment.
        /// 
        /// For example, use this to get a valid quantity given an expected minimum amount:
        ///   1.234 => 1.240 given range of [0.01 - 10.00] with increment of 0.01.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetUpperValidValue(this InclusiveRange range, decimal value)
        {
            if (value <= range.Minimum) return range.Minimum;
            if (value >= range.Maximum) return range.Maximum;

            var remainder = value % range.Increment;

            if (remainder == 0) return value;

            return value - value % range.Increment + range.Increment;
        }

        /// <summary>
        /// Get the nearest valid value within range coercing remainders to the closest increment.
        /// Specify a midpoint rounding behavior for values with remainder equals increment / 2.
        /// The default behavior rounds midpoint remainders to the nearest even increment.
        /// 
        /// For example:
        ///   1.234 => 1.230 given range of [0.01 - 10.00] with increment of 0.01.
        ///   2.345 => 2.340 given range of [0.01 - 10.00] with increment of 0.01 (midpoint rounding to even).
        ///   2.345 => 2.350 given range of [0.01 - 10.00] with increment of 0.01 (midpoint rounding away from 0).
        ///   9.876 => 9.880 given range of [0.01 - 10.00] with increment of 0.01.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetValidValue(this InclusiveRange range, decimal value, MidpointRounding midpointRounding = MidpointRounding.ToEven)
        {
            if (value <= range.Minimum) return range.Minimum;
            if (value >= range.Maximum) return range.Maximum;

            var remainder = value % range.Increment;

            if (remainder == 0) return value;

            var midpoint = range.Increment / 2;
            var lower = value - remainder;

            if (remainder < midpoint) return lower;
            if (remainder > midpoint) return lower + range.Increment;

            // Otherwise, remainder equals increment / 2...

            if (midpointRounding == MidpointRounding.AwayFromZero)
                return lower + range.Increment;

            // Round to nearest even increment...
            return (lower % (range.Increment * 2) == 0)
                ? lower // ...if lower is even.
                : lower + range.Increment;
        }

        /// <summary>
        /// Get the nearest valid value within range coercing remainders DOWN to the next increment.
        /// 
        /// For example, use this to get a valid price given an expected maximum amount:
        ///   9.876 => 9.870 given range of [0.01 - 10.00] with increment of 0.01.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetLowerValidValue(this InclusiveRange range, decimal value)
        {
            if (value <= range.Minimum) return range.Minimum;
            if (value >= range.Maximum) return range.Maximum;

            var remainder = value % range.Increment;

            if (remainder == 0) return value;

            return value - value % range.Increment;
        }
    }
}
