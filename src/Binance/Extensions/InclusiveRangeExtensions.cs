namespace Binance.Extensions
{
    public static class InclusiveRangeExtensions
    {
        /// <summary>
        /// Verify a value is within range and of a valid increment.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValid(this InclusiveRange range, decimal value)
        {
            Throw.IfNull(range, nameof(range));

            return value >= range.Minimum && value <= range.Maximum && (value - range.Minimum) % range.Increment == 0;
        }
    }
}
