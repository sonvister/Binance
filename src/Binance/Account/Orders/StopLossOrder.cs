// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class StopLossOrder : StopOrder
    {
        #region Public Properties

        public override OrderType Type => OrderType.StopLoss;

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The user (required).</param>
        public StopLossOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
