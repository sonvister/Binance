using Binance.Api;

namespace Binance.Account.Orders
{
    public abstract class StopOrder : MarketOrder, IStopOrder
    {
        #region Public Properties

        /// <summary>
        /// Get or set the stop price.
        /// </summary>
        public decimal StopPrice { get; set; }

        #endregion Public Properties

        #region Constructors

        protected StopOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
