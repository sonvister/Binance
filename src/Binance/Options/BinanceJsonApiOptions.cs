using Binance.Api.Json;

namespace Binance
{
    public sealed class BinanceJsonApiOptions
    {
        /// <summary>
        /// Receive window default (milliseconds).
        /// </summary>
        public long RecvWindowDefault { get; set; } = default;

        /// <summary>
        /// Rate limiter default count.
        /// </summary>
        public int RateLimiterCountDefault { get; set; } = RateLimiter.CountDefault;

        /// <summary>
        /// Rate limiter default duration (seconds).
        /// </summary>
        public int RateLimiterDurationSecondsDefault { get; set; } = RateLimiter.DurationSecondsDefault;

        /// <summary>
        /// Timestamp offset refresh period (minutes).
        /// </summary>
        public int TimestampOffsetRefreshPeriodMinutes { get; set; } = BinanceJsonApi.TimestampOffsetRefreshPeriodMinutesDefault;
    }
}
