using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// An account trade.
    /// </summary>
    public sealed class AccountTrade : Trade, IEquatable<AccountTrade>
    {
        #region Public Properties

        /// <summary>
        /// The order ID.
        /// </summary>
        public long OrderId { get; }

        /// <summary>
        /// Get the quote quantity.
        /// </summary>
        public decimal QuoteQuantity { get; }

        /// <summary>
        /// Get the commission (commission asset quantity).
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
        /// <param name="quoteQuantity"></param>
        /// <param name="commission"></param>
        /// <param name="commissionAsset"></param>
        /// <param name="time"></param>
        /// <param name="isBuyer"></param>
        /// <param name="isMaker"></param>
        /// <param name="isBestPriceMatch"></param>
        public AccountTrade(
            string symbol,
            long id,
            long orderId,
            decimal price,
            decimal quantity,
            decimal quoteQuantity,
            decimal commission,
            string commissionAsset,
            DateTime time,
            bool isBuyer,
            bool isMaker,
            bool isBestPriceMatch)
            : base(symbol, id, price, quantity, isBuyer ? orderId : BinanceApi.NullId, !isBuyer ? orderId : BinanceApi.NullId, time, !(isBuyer ^ isMaker), isBestPriceMatch)
        {
            if (orderId < 0)
                throw new ArgumentException($"{nameof(Trade)}: ID must not be less than 0.", nameof(orderId));

            OrderId = orderId;
            QuoteQuantity = quoteQuantity;
            Commission = commission;
            CommissionAsset = commissionAsset;
            IsBuyer = isBuyer;
            IsMaker = isMaker;
        }

        #endregion Constructors

        #region IEquatable

        public bool Equals(AccountTrade other)
        {
            if (other == null)
                return false;

            return base.Equals(other)
                && other.OrderId == OrderId
                && other.QuoteQuantity == QuoteQuantity
                && other.Commission == Commission
                && other.CommissionAsset == CommissionAsset
                && other.IsBuyer == IsBuyer
                && other.IsMaker == IsMaker;
        }

        #endregion IEquatable
    }
}
