// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class BinanceJsonApiOptions
    {
        #region Public Constants

        public static readonly int RequestRateLimitCountDefault = 1200;
        public static readonly int RequestRateLimitDurationMinutesDefault = 1;

        public static readonly int RequestRateLimitBurstCountDefault = 100;
        public static readonly int RequestRateLimitBurstDurationSecondsDefault = 1;

        public const int TimestampOffsetRefreshPeriodMinutesDefault = 60;

        #endregion Public Constants

        #region Public Properties

        /// <summary>
        /// Receive window default (milliseconds).
        /// </summary>
        public long? RecvWindowDefault { get; set; } = default;

        /// <summary>
        /// Get or set the default rate limit count for queries.
        /// </summary>
        public int RequestRateLimitCount { get; set; } = RequestRateLimitCountDefault;

        /// <summary>
        /// Get or set the default rate limit duration for queries (minutes).
        /// </summary>
        public int RequestRateLimitDurationMinutes { get; set; } = RequestRateLimitDurationMinutesDefault;

        /// <summary>
        /// Get or set the default burst rate limit count for queries.
        /// </summary>
        public int RequestRateLimitBurstCount { get; set; } = RequestRateLimitBurstCountDefault;

        /// <summary>
        /// Get or set the default burst rate limit duration for queries (seconds).
        /// </summary>
        public int RequestRateLimitBurstDurationSeconds { get; set; } = RequestRateLimitBurstDurationSecondsDefault;

        /// <summary>
        /// Timestamp offset refresh period (minutes).
        /// </summary>
        public int TimestampOffsetRefreshPeriodMinutes { get; set; } = TimestampOffsetRefreshPeriodMinutesDefault;

        #endregion Public Properties
    }
}
