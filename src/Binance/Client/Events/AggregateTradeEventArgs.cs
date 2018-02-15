using System;
using System.Threading;
using Binance.Market;

namespace Binance.Client.Events
{
    /// <summary>
    /// Aggregate trade client event arguments.
    /// </summary>
    public sealed class AggregateTradeEventArgs : ClientEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the aggregate trade.
        /// </summary>
        public AggregateTrade Trade { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="trade">The aggregate trade.</param>
        public AggregateTradeEventArgs(DateTime time, CancellationToken token, AggregateTrade trade)
            : base(time, token)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
        }

        #endregion Constructors
    }
}
