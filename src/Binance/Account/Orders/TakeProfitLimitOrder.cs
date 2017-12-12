using Binance.Api;

namespace Binance.Account.Orders
{
    public sealed class TakeProfitLimitOrder : StopLimitOrder
    {
        #region Public Properties

        public override OrderType Type => OrderType.TakeProfitLimit;

        #endregion Public Properties

        #region Constructors

        public TakeProfitLimitOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
