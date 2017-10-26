namespace Binance.Options
{
    public sealed class BinanceJsonApiOptions
    {
        /// <summary>
        /// Receive window default (milliseconds).
        /// </summary>
        public long RecvWindowDefault { get; set; }

        /// <summary>
        /// Rate limiter default count.
        /// </summary>
        public int RateLimiterCountDefault { get; set; }

        /// <summary>
        /// Rate limiter default duration (seconds).
        /// </summary>
        public int RateLimiterDurationSecondsDefault { get; set; }
    }
}
