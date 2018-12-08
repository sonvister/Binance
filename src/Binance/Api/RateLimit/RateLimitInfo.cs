// ReSharper disable once CheckNamespace
namespace Binance.Api
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
        /// Get the number of intervals.
        /// </summary>
        public int IntervalNum { get; }

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
        /// <param name="intervalNum"></param>
        /// <param name="limit"></param>
        public RateLimitInfo(string type, string interval, int intervalNum, int limit)
        {
            Throw.IfNullOrWhiteSpace(type, nameof(type));
            Throw.IfNullOrWhiteSpace(interval, nameof(interval));

            Type = type;
            Interval = interval;
            IntervalNum = intervalNum;
            Limit = limit;
        }

        #endregion Constructors
    }
}
