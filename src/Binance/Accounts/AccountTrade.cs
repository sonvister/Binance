using Binance.Trades;

namespace Binance.Accounts
{
    /// <summary>
    /// An account trade.
    /// </summary>
    public sealed class AccountTrade : Trade
    {
        #region Public Properties

        /// <summary>
        /// Get the commission.
        /// </summary>
        public decimal Commission { get; private set; }

        /// <summary>
        /// Get the commission asset.
        /// </summary>
        public string CommissionAsset { get; private set; }

        /// <summary>
        /// Get is buyer flag.
        /// </summary>
        public bool IsBuyer { get; private set; }

        /// <summary>
        /// Get is maker flag.
        /// </summary>
        public bool IsMaker { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="id"></param>
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
            decimal price,
            decimal quantity,
            decimal commission,
            string commissionAsset,
            long timestamp,
            bool isBuyer,
            bool isMaker,
            bool isBestPriceMatch)
            : base(symbol, id, price, quantity, timestamp, isBestPriceMatch)
        {
            Commission = commission;
            CommissionAsset = commissionAsset;
            IsBuyer = isBuyer;
            IsMaker = isMaker;
        }

        #endregion Constructors
    }
}
