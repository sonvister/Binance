using System;

namespace Binance.Market
{
    public sealed class AggregateTrade : Trade
    {
        #region Public Properties

        /// <summary>
        /// Get the first trade ID.
        /// </summary>
        public long FirstTradeId { get; }

        /// <summary>
        /// Get the last trade ID.
        /// </summary>
        public long LastTradeId { get; }

        /// <summary>
        /// Get flag indicating if the buyer the maker.
        /// </summary>
        public bool IsBuyerMaker { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="id">The ID.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
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
            : base(symbol, id, price, quantity, timestamp, isBestPriceMatch)
        {
            if (firstTradeId < 0)
                throw new ArgumentException($"{nameof(AggregateTrade)} trade ID must not be less than 0.", nameof(firstTradeId));
            if (lastTradeId < 0)
                throw new ArgumentException($"{nameof(AggregateTrade)} trade ID must not be less than 0.", nameof(lastTradeId));
            if (lastTradeId < firstTradeId)
                throw new ArgumentException($"{nameof(AggregateTrade)} last trade ID must be greater than or equal to first trade ID.", nameof(lastTradeId));

            FirstTradeId = firstTradeId;
            LastTradeId = lastTradeId;
            IsBuyerMaker = isBuyerMaker;
        }

        #endregion Constructors
    }
}
