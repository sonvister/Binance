// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class TakeProfitOrder : StopOrder
    {
        #region Public Properties

        public override OrderType Type => OrderType.TakeProfit;

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The user (required).</param>
        public TakeProfitOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
