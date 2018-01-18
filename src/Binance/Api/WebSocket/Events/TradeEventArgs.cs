using System.Threading;
using Binance.Market;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Trade web socket client event.
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
        /// <param name="timestamp">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="trade">The trade.</param>
        public TradeEventArgs(long timestamp, CancellationToken token, Trade trade)
            : base(timestamp, token)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
        }

        #endregion Constructors
    }
}
