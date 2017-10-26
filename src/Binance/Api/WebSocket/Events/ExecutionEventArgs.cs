using Binance.Orders;
using System;

namespace Binance.Api.WebSocket.Events
{
    public abstract class ExecutionEventArgs : EventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the event time.
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// Get the order.
        /// </summary>
        public Order Order { get; private set; }

        /// <summary>
        /// Get the order execution type.
        /// </summary>
        public OrderExecutionType OrderExecutionType { get; private set; }

        /// <summary>
        /// Get the order rejected reason.
        /// </summary>
        public OrderRejectedReason OrderRejectedReason { get; private set; }

        /// <summary>
        /// Get the new client order ID.
        /// </summary>
        public string NewClientOrderId { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="order">The order.</param>
        /// <param name="orderExecutionType">The order execution type.</param>
        /// <param name="orderRejectedReason">The order rejected reason.</param>
        /// <param name="newClientOrderId">The new client order ID.</param>
        public ExecutionEventArgs(long timestamp, Order order, OrderExecutionType orderExecutionType, OrderRejectedReason orderRejectedReason, string newClientOrderId)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(OrderUpdateEventArgs)} timestamp must be greater than 0.", nameof(timestamp));

            Throw.IfNull(order, nameof(order));

            Timestamp = timestamp;
            Order = order;
            OrderExecutionType = orderExecutionType;
            OrderRejectedReason = orderRejectedReason;
            NewClientOrderId = newClientOrderId;
        }

        #endregion Constructors
    }
}
