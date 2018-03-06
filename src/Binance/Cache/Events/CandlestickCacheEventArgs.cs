using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public sealed class CandlestickCacheEventArgs : CacheEventArgs
    {
        #region Public Properties

        /// <summary>
        /// The candlesticks.
        /// </summary>
        public IEnumerable<Candlestick> Candlesticks { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="candlesticks">The candlesticks.</param>
        public CandlestickCacheEventArgs(IEnumerable<Candlestick> candlesticks)
        {
            Throw.IfNull(candlesticks, nameof(candlesticks));

            Candlesticks = candlesticks;
        }

        #endregion Constructors
    }
}
