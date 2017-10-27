using System;
using System.Collections.Generic;

namespace Binance.Trades.Cache
{
    public sealed class AggregateTradesCacheEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// The latest trades.
        /// </summary>
        public IEnumerable<AggregateTrade> Trades { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trades">The latest trades.</param>
        public AggregateTradesCacheEventArgs(IEnumerable<AggregateTrade> trades)
        {
            Throw.IfNull(trades, nameof(trades));

            Trades = trades;
        }

        #endregion Constructors
    }
}
