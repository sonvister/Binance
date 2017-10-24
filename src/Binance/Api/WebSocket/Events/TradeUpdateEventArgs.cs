using Binance.Accounts;
using Binance.Orders;

namespace Binance.Api.WebSocket.Events
{
    public sealed class TradeUpdateEventArgs : OrderUpdateEventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the trade.
        /// </summary>
        public AccountTrade Trade { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="trade">The trade.</param>
        public TradeUpdateEventArgs(long timestamp, Order order, OrderExecutionType executionType, OrderRejectedReason rejectedReason, AccountTrade trade)
            : base(timestamp, order, executionType, rejectedReason)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
        }

        #endregion Constructors
    }
}
