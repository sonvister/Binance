// ReSharper disable once CheckNamespace
namespace Binance
{
    public abstract class ClientOrder
    {
        #region Public Properties

        /// <summary>
        /// Get the user.
        /// </summary>
        public IBinanceApiUser User { get; }

        /// <summary>
        /// Get or set the symbol.
        /// </summary>
        public virtual string Symbol { get; set; }

        /// <summary>
        /// Get the order type.
        /// </summary>
        public abstract OrderType Type { get; }

        /// <summary>
        /// Get or set the order side.
        /// </summary>
        public virtual OrderSide? Side { get; set; }

        /// <summary>
        /// Get or set the quantity.
        /// </summary>
        public virtual decimal Quantity { get; set; }

        /// <summary>
        /// Get or set the client order ID (newClientOrderId).
        /// NOTE: This value is set internally after order placement.
        /// </summary>
        public virtual string Id { get; set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The user (required).</param>
        protected ClientOrder(IBinanceApiUser user)
        {
            Throw.IfNull(user, nameof(user));

            User = user;
        }

        #endregion Constructors
    }
}
