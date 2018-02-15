using System;
using System.Threading;
using Binance.Account.Orders;

namespace Binance.Client.Events
{
    /// <summary>
    /// Order execution event arguments.
    /// </summary>
    public abstract class OrderExecutionEventArgs : UserDataEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the order.
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Get the order execution type.
        /// </summary>
        public OrderExecutionType OrderExecutionType { get; }

        /// <summary>
        /// Get the order rejected reason.
        /// </summary>
        public OrderRejectedReason OrderRejectedReason { get; }

        /// <summary>
        /// Get the new client order ID.
        /// </summary>
        public string NewClientOrderId { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="order">The order.</param>
        /// <param name="orderExecutionType">The order execution type.</param>
        /// <param name="orderRejectedReason">The order rejected reason.</param>
        /// <param name="newClientOrderId">The new client order ID.</param>
        protected OrderExecutionEventArgs(DateTime time, CancellationToken token, Order order, OrderExecutionType orderExecutionType, OrderRejectedReason orderRejectedReason, string newClientOrderId)
            : base(time, token)
        {
            Throw.IfNull(order, nameof(order));

            Order = order;
            OrderExecutionType = orderExecutionType;
            OrderRejectedReason = orderRejectedReason;
            NewClientOrderId = newClientOrderId;
        }

        #endregion Constructors
    }
}
