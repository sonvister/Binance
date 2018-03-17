// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class TakeProfitLimitOrder : StopLimitOrder
    {
        #region Public Properties

        public override OrderType Type => OrderType.TakeProfitLimit;

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The user (required).</param>
        public TakeProfitLimitOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
