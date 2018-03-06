using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public sealed class AggregateTradeCacheEventArgs : CacheEventArgs
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
