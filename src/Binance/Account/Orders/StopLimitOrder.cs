// ReSharper disable once CheckNamespace
namespace Binance
{
    public abstract class StopLimitOrder : LimitOrder, IStopOrder
    {
        #region Public Properties

        /// <summary>
        /// Get or set the stop price.
        /// </summary>
        public virtual decimal StopPrice { get; set; }

        #endregion Public Properties

        #region Constructors

        protected StopLimitOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
