namespace Binance.Account.Orders
{
    public enum OrderStatus
    {
        /// <summary>
        /// New order status.
        /// </summary>
        New,

        /// <summary>
        /// Partially filled order status.
        /// </summary>
        PartiallyFilled,

        /// <summary>
        /// Filled order status.
        /// </summary>
        Filled,

        /// <summary>
        /// Canceled order status.
        /// </summary>
        Canceled,

        /// <summary>
        /// Pending cancel order status.
        /// </summary>
        PendingCancel,

        /// <summary>
        /// Rejected order status.
        /// </summary>
        Rejected,

        /// <summary>
        /// Expired order status.
        /// </summary>
        Expired
    }
}
