namespace Binance.Api.RateLimit
{
    public sealed class RateLimitInfo
    {
        #region Public Properties

        /// <summary>
        /// Get the type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Get the interval.
        /// </summary>
        public string Interval { get; }

        /// <summary>
        /// Get the limit.
        /// </summary>
        public int Limit { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        public RateLimitInfo(string type, string interval, int limit)
        {
            Throw.IfNullOrWhiteSpace(type, nameof(type));
            Throw.IfNullOrWhiteSpace(interval, nameof(interval));

            Type = type;
            Interval = interval;
            Limit = limit;
        }

        #endregion Constructors
    }
}
