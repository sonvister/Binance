using Binance.Api.Json;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class BinanceJsonApiOptions
    {
        /// <summary>
        /// Receive window default (milliseconds).
        /// </summary>
        public long RecvWindowDefault { get; set; } = default;

        /// <summary>
        /// Get or set the default rate limit count for queries.
        /// </summary>
        public int QueryRateLimitCount { get; set; } = BinanceJsonApi.QueryRateLimitCountDefault;

        /// <summary>
        /// Get or set the default rate limit duration for queries (minutes).
        /// </summary>
        public int QueryRateLimitDurationMinutes { get; set; } = BinanceJsonApi.QueryRateLimitDurationMinutesDefault;

        /// <summary>
        /// Get or set the default burst rate limit count for queries.
        /// </summary>
        public int QueryRateLimitBurstCount { get; set; } = BinanceJsonApi.QueryRateLimitBurstCountDefault;

        /// <summary>
        /// Get or set the default burst rate limit duration for queries (seconds).
        /// </summary>
        public int QueryRateLimitBurstDurationSeconds { get; set; } = BinanceJsonApi.QueryRateLimitBurstDurationSecondsDefault;

        /// <summary>
        /// Get or set the default rate limit count for orders.
        /// </summary>
        public int OrderRateLimitCount { get; set; } = BinanceJsonApi.OrderRateLimitCountDefault;

        /// <summary>
        /// Get or set the default rate limit duration for orders (seconds).
        /// </summary>
        public int OrderRateLimitDurationDays { get; set; } = BinanceJsonApi.OrderRateLimitDurationDaysDefault;

        /// <summary>
        /// Get or set the default burst rate limit count for orders.
        /// </summary>
        public int OrderRateLimitBurstCount { get; set; } = BinanceJsonApi.OrderRateLimitBurstCountDefault;

        /// <summary>
        /// Get or set the default burst rate limit duration for orders (seconds).
        /// </summary>
        public int OrderRateLimitBurstDurationSeconds { get; set; } = BinanceJsonApi.OrderRateLimitBurstDurationSecondsDefault;

        /// <summary>
        /// Timestamp offset refresh period (minutes).
        /// </summary>
        public int TimestampOffsetRefreshPeriodMinutes { get; set; } = BinanceJsonApi.TimestampOffsetRefreshPeriodMinutesDefault;
    }
}
