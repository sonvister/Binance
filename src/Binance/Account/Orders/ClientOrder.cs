using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public abstract class ClientOrder : IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the user.
        /// </summary>
        public IBinanceApiUser User { get; }

        /// <summary>
        /// Get or set the symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Get the order type.
        /// </summary>
        public abstract OrderType Type { get; }

        /// <summary>
        /// Get or set the order side.
        /// </summary>
        public OrderSide Side { get; set; }

        /// <summary>
        /// Get or set the quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Get or set the client order ID (newClientOrderId).
        /// NOTE: This value is set internally after order placement.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Get the transact time.
        /// NOTE: This value is set internally after order placement.
        /// </summary>
        public DateTime Time { get; internal set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user"></param>
        protected ClientOrder(IBinanceApiUser user)
        {
            Throw.IfNull(user, nameof(user));

            User = user;
        }

        #endregion Constructors
    }
}
