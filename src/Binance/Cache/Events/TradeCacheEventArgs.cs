using System;
using System.Collections.Generic;
using Binance.Market;

namespace Binance.Cache.Events
{
    public sealed class TradeCacheEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// The latest trades.
        /// </summary>
        public IEnumerable<Trade> Trades { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trades">The latest trades.</param>
        public TradeCacheEventArgs(IEnumerable<Trade> trades)
        {
            Throw.IfNull(trades, nameof(trades));

            Trades = trades;
        }

        #endregion Constructors
    }
}
