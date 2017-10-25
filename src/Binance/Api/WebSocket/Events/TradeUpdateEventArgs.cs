using Binance.Accounts;
using Binance.Orders;

namespace Binance.Api.WebSocket.Events
{
    public sealed class TradeUpdateEventArgs : ExecutionEventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the trade.
        /// </summary>
        public AccountTrade Trade { get; private set; }

        /// <summary>
        /// Get the quantity of the last filled trade
        /// </summary>
        public decimal QuantityOfLastFilledTrade { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="order">The order.</param>
        /// <param name="executionType">The order execution type.</param>
        /// <param name="rejectedReason">The order rejected reason.</param>
        /// <param name="newClientOrderId">The new client order ID.</param>
        /// <param name="trade">The trade.</param>
        /// <param name="quantityOfLastFilledTrade">The quantity of last filled trade.</param>
        public TradeUpdateEventArgs(long timestamp, Order order, OrderExecutionType executionType, OrderRejectedReason rejectedReason, string newClientOrderId, AccountTrade trade, decimal quantityOfLastFilledTrade)
            : base(timestamp, order, executionType, rejectedReason, newClientOrderId)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
            QuantityOfLastFilledTrade = quantityOfLastFilledTrade;
        }

        #endregion Constructors
    }
}
