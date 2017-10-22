using Binance.Trades;
using System;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Depth web socket client event.
    /// </summary>
    public sealed class AggregateTradeEventArgs : EventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the event time.
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// Get the aggregate trade.
        /// </summary>
        public AggregateTrade Trade { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="trade">The aggregate trade.</param>
        public AggregateTradeEventArgs(long timestamp, AggregateTrade trade)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(AggregateTradeEventArgs)}: Event {nameof(timestamp)} must be greater than 0.", nameof(timestamp));

            Throw.IfNull(trade, nameof(trade));

            Timestamp = timestamp;
            Trade = trade;
        }

        #endregion Constructors
    }
}
