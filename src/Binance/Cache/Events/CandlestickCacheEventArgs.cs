using System;
using System.Collections.Generic;
using Binance.Market;

namespace Binance.Cache.Events
{
    public sealed class CandlestickCacheEventArgs : EventArgs
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
