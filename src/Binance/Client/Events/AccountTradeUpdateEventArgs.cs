using System;
using Binance.Account;
using Binance.Account.Orders;

namespace Binance.Client.Events
{
    /// <summary>
    /// Account trade update event arguments.
    /// </summary>
    public sealed class AccountTradeUpdateEventArgs : OrderExecutionEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the trade.
        /// </summary>
        public AccountTrade Trade { get; }

        /// <summary>
        /// Get the quantity of the last filled trade
        /// </summary>
        public decimal QuantityOfLastFilledTrade { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="order">The order.</param>
        /// <param name="rejectedReason">The order rejected reason.</param>
        /// <param name="newClientOrderId">The new client order ID.</param>
        /// <param name="trade">The trade.</param>
        /// <param name="quantityOfLastFilledTrade">The quantity of last filled trade.</param>
        public AccountTradeUpdateEventArgs(DateTime time, Order order, string rejectedReason, string newClientOrderId, AccountTrade trade, decimal quantityOfLastFilledTrade)
            : base(time, order, OrderExecutionType.Trade, rejectedReason, newClientOrderId)
        {
            Throw.IfNull(trade, nameof(trade));

            Trade = trade;
            QuantityOfLastFilledTrade = quantityOfLastFilledTrade;
        }

        #endregion Constructors
    }
}
