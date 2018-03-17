using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class LimitMakerOrder : LimitOrder
    {
        #region Public Properties

        public override OrderType Type => OrderType.LimitMaker;

        public override TimeInForce TimeInForce
        {
            get => TimeInForce.GTC;
            set
            {
                if (value != TimeInForce.GTC)
                    throw new ArgumentException($"{nameof(TimeInForce)} must be {TimeInForce.GTC}.", nameof(TimeInForce));
            }
        }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The user (required).</param>
        public LimitMakerOrder(IBinanceApiUser user)
            : base(user)
        { }

        #endregion Constructors
    }
}
