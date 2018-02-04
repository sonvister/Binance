// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class BinanceApiOptions
    {
        #region Public Constants

        public static readonly int RequestRateLimitCountDefault = 1200;
        public static readonly int RequestRateLimitDurationMinutesDefault = 1;

        public static readonly int RequestRateLimitBurstCountDefault = 100;
        public static readonly int RequestRateLimitBurstDurationSecondsDefault = 1;

        public static readonly int OrderRateLimitCountDefault = 100000;
        public static readonly int OrderRateLimitDurationDaysDefault = 1;

        public static readonly int OrderRateLimitBurstCountDefault = 10;
        public static readonly int OrderRateLimitBurstDurationSecondsDefault = 1;

        public const int TimestampOffsetRefreshPeriodMinutesDefault = 30;

        #endregion Public Constants

        #region Public Properties

        /// <summary>
        /// Get or set the service point manager connection least timeout
        /// Workaround for: "Singleton HttpClient doesn't respect DNS changes."
        /// https://github.com/dotnet/corefx/issues/11224
        /// </summary>
        public int ServicePointManagerConnectionLeaseTimeout { get; set; } = 60 * 1000; // 1 minute.

        /// <summary>
        /// Receive window default (milliseconds).
        /// </summary>
        public long? RecvWindowDefault { get; set; } = default;

        /// <summary>
        /// Timestamp offset refresh period (minutes).
        /// </summary>
        public int TimestampOffsetRefreshPeriodMinutes { get; set; } = TimestampOffsetRefreshPeriodMinutesDefault;

        public class RequestRateLimitOptions
        {
            /// <summary>
            /// Get or set the default rate limit count for queries.
            /// </summary>
            public int Count { get; set; } = RequestRateLimitCountDefault;

            /// <summary>
            /// Get or set the default rate limit duration for queries (minutes).
            /// </summary>
            public int DurationMinutes { get; set; } = RequestRateLimitDurationMinutesDefault;

            /// <summary>
            /// Get or set the default burst rate limit count for queries.
            /// </summary>
            public int BurstCount { get; set; } = RequestRateLimitBurstCountDefault;

            /// <summary>
            /// Get or set the default burst rate limit duration for queries (seconds).
            /// </summary>
            public int BurstDurationSeconds { get; set; } = RequestRateLimitBurstDurationSecondsDefault;
        }

        public RequestRateLimitOptions RequestRateLimit { get; } = new RequestRateLimitOptions();

        public class OrderRateLimitOptions
        {
            /// <summary>
            /// Get or set the default rate limit count for orders.
            /// </summary>
            public int Count { get; set; } = OrderRateLimitCountDefault;

            /// <summary>
            /// Get or set the default rate limit duration for orders (days).
            /// </summary>
            public int DurationDays { get; set; } = OrderRateLimitDurationDaysDefault;

            /// <summary>
            /// Get or set the default burst rate limit count for orders.
            /// </summary>
            public int BurstCount { get; set; } = OrderRateLimitBurstCountDefault;

            /// <summary>
            /// Get or set the default burst rate limit duration for orders (seconds).
            /// </summary>
            public int BurstDurationSeconds { get; set; } = OrderRateLimitBurstDurationSecondsDefault;
        }

        public OrderRateLimitOptions OrderRateLimit { get; } = new OrderRateLimitOptions();

        #endregion Public Properties
    }
}
