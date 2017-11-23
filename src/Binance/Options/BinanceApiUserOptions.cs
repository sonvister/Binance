// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class BinanceApiUserOptions
    {
        #region Public Constants

        public static readonly int OrderRateLimitCountDefault = 100000;
        public static readonly int OrderRateLimitDurationDaysDefault = 1;

        public static readonly int OrderRateLimitBurstCountDefault = 10;
        public static readonly int OrderRateLimitBurstDurationSecondsDefault = 1;

        #endregion Public Constants

        #region Public Properties

        /// <summary>
        /// Get or set the default rate limit count for orders.
        /// </summary>
        public int OrderRateLimitCount { get; set; } = OrderRateLimitCountDefault;

        /// <summary>
        /// Get or set the default rate limit duration for orders (seconds).
        /// </summary>
        public int OrderRateLimitDurationDays { get; set; } = OrderRateLimitDurationDaysDefault;

        /// <summary>
        /// Get or set the default burst rate limit count for orders.
        /// </summary>
        public int OrderRateLimitBurstCount { get; set; } = OrderRateLimitBurstCountDefault;

        /// <summary>
        /// Get or set the default burst rate limit duration for orders (seconds).
        /// </summary>
        public int OrderRateLimitBurstDurationSeconds { get; set; } = OrderRateLimitBurstDurationSecondsDefault;

        #endregion Public Properties
    }
}
