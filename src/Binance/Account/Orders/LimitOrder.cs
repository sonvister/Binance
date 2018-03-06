// ReSharper disable once CheckNamespace
namespace Binance
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
        public virtual decimal Price { get; set; }

        /// <summary>
        /// Get or set the iceberg quantity.
        /// </summary>
        public virtual decimal IcebergQuantity { get; set; }

        /// <summary>
        /// Get or set the time in force.
        /// </summary>
        public virtual TimeInForce TimeInForce { get; set; }

        #endregion Public Properties

        #region Constructors

        public LimitOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
