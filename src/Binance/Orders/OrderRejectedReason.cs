namespace Binance.Orders
{
    public enum OrderRejectedReason
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Unknown instrument.
        /// </summary>
        UnknownInstrument,
        
        /// <summary>
        /// Market closed.
        /// </summary>
        MarketClosed,

        /// <summary>
        /// Price-Quantity exceeds hard limits.
        /// </summary>
        PriceQuantityExceedHardLimits,

        /// <summary>
        /// Unknown order.
        /// </summary>
        UnknownOrder,

        /// <summary>
        /// Duplicate order.
        /// </summary>
        DuplicateOrder,

        /// <summary>
        /// Unknown account.
        /// </summary>
        UnknownAccount,

        /// <summary>
        /// Insufficient balance.
        /// </summary>
        InsufficientBalance,

        /// <summary>
        /// Account inactive.
        /// </summary>
        AccountInactive,

        /// <summary>
        /// Account cannot settle.
        /// </summary>
        AccountCannotSettle
    }
}
