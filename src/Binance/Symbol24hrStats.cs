namespace Binance
{
    /// <summary>
    /// Symbol 24-hour statistics.
    /// </summary>
    public sealed class Symbol24hrStats
    {
        #region Public Properties

        /// <summary>
        /// The symbol.
        /// </summary>
        public string Symbol { get; private set; }

        /// <summary>
        /// The price change amount.
        /// </summary>
        public decimal PriceChange { get; private set; }

        /// <summary>
        /// The price change percent
        /// </summary>
        public decimal PriceChangePercent { get; private set; }

        /// <summary>
        /// The weighted average price.
        /// </summary>
        public decimal WeightedAveragePrice { get; private set; }

        /// <summary>
        /// The previous close price.
        /// </summary>
        public decimal PreviousClosePrice { get; private set; }

        /// <summary>
        /// The last price.
        /// </summary>
        public decimal LastPrice { get; private set; }

        /// <summary>
        /// The bid price.
        /// </summary>
        public decimal BidPrice { get; private set; }

        /// <summary>
        /// The ask price
        /// </summary>
        public decimal AskPrice { get; private set; }

        /// <summary>
        /// The open price.
        /// </summary>
        public decimal OpenPrice { get; private set; }

        /// <summary>
        /// The high price.
        /// </summary>
        public decimal HighPrice { get; private set; }

        /// <summary>
        /// The low price.
        /// </summary>
        public decimal LowPrice { get; private set; }

        /// <summary>
        /// The volume.
        /// </summary>
        public decimal Volume { get; private set; }

        /// <summary>
        /// The open time.
        /// </summary>
        public decimal OpenTime { get; private set; }

        /// <summary>
        /// The close time.
        /// </summary>
        public decimal CloseTime { get; private set; }

        /// <summary>
        /// The first trade ID.
        /// </summary>
        public long FirstTradeId { get; private set; }

        /// <summary>
        /// The last trade ID.
        /// </summary>
        public long LastTradeId { get; private set; }

        /// <summary>
        /// The trade count.
        /// </summary>
        public long TradeCount { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="priceChange"></param>
        /// <param name="priceChangePercent"></param>
        /// <param name="weightedAveragePrice"></param>
        /// <param name="previousClosePrice"></param>
        /// <param name="lastPrice"></param>
        /// <param name="bidPrice"></param>
        /// <param name="askPrice"></param>
        /// <param name="openPrice"></param>
        /// <param name="highPrice"></param>
        /// <param name="lowPrice"></param>
        /// <param name="volume"></param>
        /// <param name="openTime"></param>
        /// <param name="closeTime"></param>
        /// <param name="firstTradeId"></param>
        /// <param name="lastTradeId"></param>
        /// <param name="tradeCount"></param>
        public Symbol24hrStats(
            string symbol,
            decimal priceChange,
            decimal priceChangePercent,
            decimal weightedAveragePrice,
            decimal previousClosePrice,
            decimal lastPrice,
            decimal bidPrice,
            decimal askPrice,
            decimal openPrice,
            decimal highPrice,
            decimal lowPrice,
            decimal volume,
            long openTime,
            long closeTime,
            long firstTradeId,
            long lastTradeId,
            long tradeCount)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol;

            PriceChange = priceChange;
            PriceChangePercent = priceChangePercent;
            WeightedAveragePrice = weightedAveragePrice;
            PreviousClosePrice = previousClosePrice;
            LastPrice = lastPrice;
            BidPrice = bidPrice;
            AskPrice = askPrice;
            OpenPrice = openPrice;
            HighPrice = highPrice;
            LowPrice = lowPrice;
            Volume = volume;
            OpenTime = openTime;
            CloseTime = closeTime;
            FirstTradeId = firstTradeId;
            LastTradeId = lastTradeId;
            TradeCount = tradeCount;
        }

        #endregion Constructors
    }
}
