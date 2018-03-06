using System;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    /// <summary>
    /// Order update event arguments.
    /// </summary>
    public sealed class OrderUpdateEventArgs : OrderExecutionEventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="order">The order.</param>
        /// <param name="executionType">The order execution type.</param>
        /// <param name="rejectedReason">The order rejected reason.</param>
        /// <param name="newClientOrderId">The new client order ID.</param>
        public OrderUpdateEventArgs(DateTime time, Order order, OrderExecutionType executionType, string rejectedReason, string newClientOrderId)
            : base(time, order, executionType, rejectedReason, newClientOrderId)
        { }

        #endregion Constructors
    }
}
