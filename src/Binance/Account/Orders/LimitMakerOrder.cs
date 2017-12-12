using Binance.Api;

namespace Binance.Account.Orders
{
    public sealed class LimitMakerOrder : LimitOrder
    {
        #region Public Properties

        public override OrderType Type => OrderType.LimitMaker;

        #endregion Public Properties

        #region Constructors

        public LimitMakerOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
