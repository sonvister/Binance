// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// Remove trailing zeros.
        /// </summary>
        /// <param name="value">The decimal value to normalize.</param>
        /// <returns></returns>
        public static decimal Normalize(this decimal value)
            => value / 1.0000000000000000000000000000m;
    }
}
