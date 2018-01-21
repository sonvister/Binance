using System.Threading;
using Binance.Account.Orders;

namespace Binance.WebSocket.Events
{
    public sealed class OrderUpdateEventArgs : OrderExecutionEventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="order">The order.</param>
        /// <param name="executionType">The order execution type.</param>
        /// <param name="rejectedReason">The order rejected reason.</param>
        /// <param name="newClientOrderId">The new client order ID.</param>
        public OrderUpdateEventArgs(long timestamp, CancellationToken token, Order order, OrderExecutionType executionType, OrderRejectedReason rejectedReason, string newClientOrderId)
            : base(timestamp, token, order, executionType, rejectedReason, newClientOrderId)
        { }

        #endregion Constructors
    }
}
