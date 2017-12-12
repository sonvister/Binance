using Binance.Api;

namespace Binance.Account.Orders
{
    public class LimitOrder : ClientOrder
    {
        #region Public Properties

        /// <summary>
        /// Get the order type.
        /// </summary>
        public override OrderType Type => OrderType.Limit;

        /// <summary>
        /// Get or set the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Get or set the iceberg quantity.
        /// </summary>
        public decimal IcebergQuantity { get; set; }

        /// <summary>
        /// Get or set the time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; set; }

        #endregion Public Properties

        #region Constructors

        public LimitOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
