using System;

namespace Binance.Market
{
    /// <summary>
    /// Trades that fill at the same time, from the same order, with the same
    /// price will have an aggregate quantity.
    /// </summary>
    public sealed class AggregateTrade : IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the aggregate trade ID.
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

        /// <summary>
        /// Get the first trade ID.
        /// </summary>
        public long FirstTradeId { get; }

        /// <summary>
        /// Get the last trade ID.
        /// </summary>
        public long LastTradeId { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="id">The ID.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The aggregate quantity.</param>
        /// <param name="firstTradeId">The first trade ID.</param>
        /// <param name="lastTradeId">The last trade ID.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="isBuyerMaker">Is buyer maker.</param>
        /// <param name="isBestPriceMatch">Is best price match.</param>
        public AggregateTrade(
            string symbol,
            long id,
            decimal price,
            decimal quantity,
            long firstTradeId,
            long lastTradeId,
            long timestamp,
            bool isBuyerMaker,
            bool isBestPriceMatch)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (id < 0)
                throw new ArgumentException($"{nameof(AggregateTrade)}: ID must not be less than 0.", nameof(id));
            if (price < 0)
                throw new ArgumentException($"{nameof(AggregateTrade)}: price must not be less than 0.", nameof(price));
            if (quantity <= 0)
                throw new ArgumentException($"{nameof(AggregateTrade)}: quantity must be greater than 0.", nameof(quantity));
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(AggregateTrade)}: timestamp must be greater than 0.", nameof(timestamp));
            if (firstTradeId < 0)
                throw new ArgumentException($"{nameof(AggregateTrade)} ID must not be less than 0.", nameof(firstTradeId));
            if (lastTradeId < 0)
                throw new ArgumentException($"{nameof(AggregateTrade)} ID must not be less than 0.", nameof(lastTradeId));
            if (lastTradeId < firstTradeId)
                throw new ArgumentException($"{nameof(AggregateTrade)} last trade ID must be greater than or equal to first trade ID.", nameof(lastTradeId));

            Symbol = symbol;
            Id = id;
            Price = price;
            Quantity = quantity;
            Timestamp = timestamp;
            IsBuyerMaker = isBuyerMaker;
            IsBestPriceMatch = isBestPriceMatch;
            FirstTradeId = firstTradeId;
            LastTradeId = lastTradeId;
        }

        #endregion Constructors
    }
}
