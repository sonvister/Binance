// ReSharper disable once CheckNamespace
namespace Binance
{
    public class MarketOrder : ClientOrder
    {
        #region Public Properties

        /// <summary>
        /// Get the order type.
        /// </summary>
        public override OrderType Type => OrderType.Market;

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The user (required).</param>
        public MarketOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
