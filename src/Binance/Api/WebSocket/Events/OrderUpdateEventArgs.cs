using Binance.Orders;
using System;

namespace Binance.Api.WebSocket.Events
{
    public class OrderUpdateEventArgs : EventArgs, IChronological
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
        public OrderExecutionType ExecutionType { get; private set; }

        /// <summary>
        /// Get the order rejected reason.
        /// </summary>
        public OrderRejectedReason RejectedReason { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="order">The order.</param>
        /// <param name="executionType"></param>
        public OrderUpdateEventArgs(long timestamp, Order order, OrderExecutionType executionType, OrderRejectedReason rejectedReason)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(OrderUpdateEventArgs)} timestamp must be greater than 0.", nameof(timestamp));

            Throw.IfNull(order, nameof(order));

            Timestamp = timestamp;
            Order = order;
            ExecutionType = executionType;
            RejectedReason = rejectedReason;
        }

        #endregion Constructors
    }
}
