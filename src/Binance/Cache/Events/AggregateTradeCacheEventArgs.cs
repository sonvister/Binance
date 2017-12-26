using System;
using System.Collections.Generic;
using Binance.Market;

namespace Binance.Cache.Events
{
    public sealed class AggregateTradeCacheEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// The latest trades.
        /// </summary>
        public IEnumerable<AggregateTrade> Trades { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trades">The latest trades.</param>
        public AggregateTradeCacheEventArgs(IEnumerable<AggregateTrade> trades)
        {
            Throw.IfNull(trades, nameof(trades));

            Trades = trades;
        }

        #endregion Constructors
    }
}
