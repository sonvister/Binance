using System.Threading;
using Binance.Market;

namespace Binance.WebSocket.Events
{
    /// <summary>
    /// Aggregate trade web socket client event.
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
        /// <param name="timestamp">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="trade">The aggregate trade.</param>
        public AggregateTradeEventArgs(long timestamp, CancellationToken token, AggregateTrade trade)
            : base(timestamp, token)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
        }

        #endregion Constructors
    }
}
