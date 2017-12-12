using Binance.Api;

namespace Binance.Account.Orders
{
    public abstract class StopLimitOrder : LimitOrder, IStopOrder
    {
        #region Public Properties

        /// <summary>
        /// Get or set the stop price.
        /// </summary>
        public decimal StopPrice { get; set; }

        #endregion Public Properties

        #region Constructors

        public StopLimitOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
