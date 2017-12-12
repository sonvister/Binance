namespace Binance.Account.Orders
{
    public enum OrderType
    {
        /// <summary>
        /// Limit order.
        /// </summary>
        Limit,

        /// <summary>
        /// Market order.
        /// </summary>
        Market,

        /// <summary>
        /// Stop loss market order.
        /// </summary>
        StopLoss,

        /// <summary>
        /// Stop loss limit order.
        /// </summary>
        StopLossLimit,

        /// <summary>
        /// Take profit market order.
        /// </summary>
        TakeProfit,

        /// <summary>
        /// Take profit limit order.
        /// </summary>
        TakeProfitLimit,

        /// <summary>
        /// Limit maker order.
        /// </summary>
        LimitMaker
    }
}
