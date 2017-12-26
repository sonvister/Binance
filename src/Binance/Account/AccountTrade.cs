using System;
using Binance.Api;
using Binance.Market;

namespace Binance.Account
{
    /// <summary>
    /// An account trade.
    /// </summary>
    public sealed class AccountTrade : Trade
    {
        #region Public Properties

        /// <summary>
        /// The order ID.
        /// </summary>
        public long OrderId { get; }

        /// <summary>
        /// Get the commission.
        /// </summary>
        public decimal Commission { get; }

        /// <summary>
        /// Get the commission asset.
        /// </summary>
        public string CommissionAsset { get; }

        /// <summary>
        /// Get is buyer flag.
        /// </summary>
        public bool IsBuyer { get; }

        /// <summary>
        /// Get is maker flag.
        /// </summary>
        public bool IsMaker { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="id"></param>
        /// <param name="orderId"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="commission"></param>
        /// <param name="commissionAsset"></param>
        /// <param name="timestamp"></param>
        /// <param name="isBuyer"></param>
        /// <param name="isMaker"></param>
        /// <param name="isBestPriceMatch"></param>
        public AccountTrade(
            string symbol,
            long id,
            long orderId,
            decimal price,
            decimal quantity,
            decimal commission,
            string commissionAsset,
            long timestamp,
            bool isBuyer,
            bool isMaker,
            bool isBestPriceMatch)
            : base(symbol, id, price, quantity, BinanceApi.NullId, BinanceApi.NullId, timestamp, !(isBuyer ^ isMaker), isBestPriceMatch)
        {
            if (orderId < 0)
                throw new ArgumentException($"{nameof(Trade)}: ID must not be less than 0.", nameof(orderId));

            AccountCommissions.ThrowIfCommissionIsInvalid(commission, nameof(commission));

            OrderId = orderId;
            Commission = commission;
            CommissionAsset = commissionAsset;
            IsBuyer = isBuyer;
            IsMaker = isMaker;
        }

        #endregion Constructors
    }
}
