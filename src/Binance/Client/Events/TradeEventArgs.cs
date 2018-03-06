using System;
using Binance.Market;

namespace Binance.Client.Events
{
    /// <summary>
    /// Trade client event arguments.
    /// </summary>
    public sealed class TradeEventArgs : ClientEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the trade.
        /// </summary>
        public Trade Trade { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="trade">The trade.</param>
        public TradeEventArgs(DateTime time, Trade trade)
            : base(time)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
        }

        #endregion Constructors
    }
}
