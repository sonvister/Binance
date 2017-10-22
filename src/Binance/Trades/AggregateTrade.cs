namespace Binance.Trades
{
    public class AggregateTrade : Trade
    {
        #region Public Properties

        /// <summary>
        /// Get the first trade ID.
        /// </summary>
        public long FirstTradeId { get; private set; }

        /// <summary>
        /// Get the last trade ID.
        /// </summary>
        public long LastTradeId { get; private set; }

        /// <summary>
        /// Get flag indicating if the buyer the maker.
        /// </summary>
        public bool IsBuyerMaker { get; private set; }

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
            FirstTradeId = firstTradeId;
            LastTradeId = lastTradeId;
            IsBuyerMaker = isBuyerMaker;
        }

        #endregion Constructors
    }
}
