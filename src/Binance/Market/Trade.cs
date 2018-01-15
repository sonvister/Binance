using System;

namespace Binance.Market
{
    /// <summary>
    /// An abstract trade class.
    /// </summary>
    public class Trade : IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the trade ID.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Get the price.
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Get the quantity.
        /// </summary>
        public decimal Quantity { get; }

        /// <summary>
        /// Get the buyer order ID.
        /// </summary>
        public long BuyerOrderId { get; }

        /// <summary>
        /// Get the seller order ID.
        /// </summary>
        public long SellerOrderId { get; }

        /// <summary>
        /// Get the trade timestamp.
        /// </summary>
        public long Timestamp { get; }

        /// <summary>
        /// Get flag indicating if the buyer the maker.
        /// </summary>
        public bool IsBuyerMaker { get; }

        /// <summary>
        /// Get flag indicating if the trade was the best price match.
        /// </summary>
        public bool IsBestPriceMatch { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="id">The trade ID.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="buyerOrderId">The buyer order ID.</param>
        /// <param name="sellerOrderId">The seller order ID.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="isBuyerMaker">Is buyer maker.</param>
        /// <param name="isBestPriceMatch">Flag indicating if the trade was the best price match.</param>
        public Trade(
            string symbol,
            long id,
            decimal price,
            decimal quantity,
            long buyerOrderId,
            long sellerOrderId,
            long timestamp,
            bool isBuyerMaker,
            bool isBestPriceMatch)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (id < 0)
                throw new ArgumentException($"{nameof(Trade)}: ID must not be less than 0.", nameof(id));
            if (price < 0)
                throw new ArgumentException($"{nameof(Trade)}: price must not be less than 0.", nameof(price));
            if (quantity <= 0)
                throw new ArgumentException($"{nameof(Trade)}: quantity must be greater than 0.", nameof(quantity));
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(Trade)}: timestamp must be greater than 0.", nameof(timestamp));

            Symbol = symbol.FormatSymbol();
            Id = id;
            Price = price;
            Quantity = quantity;
            BuyerOrderId = buyerOrderId;
            SellerOrderId = sellerOrderId;
            Timestamp = timestamp;
            IsBuyerMaker = isBuyerMaker;
            IsBestPriceMatch = isBestPriceMatch;
        }

        #endregion Constructors
    }
}
