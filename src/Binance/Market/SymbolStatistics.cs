using System;

namespace Binance.Market
{
    /// <summary>
    /// Symbol 24-hour statistics.
    /// </summary>
    public sealed class SymbolStatistics
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the time period.
        /// </summary>
        public TimeSpan Period { get; }

        /// <summary>
        /// Get the price change amount.
        /// </summary>
        public decimal PriceChange { get; }

        /// <summary>
        /// Get the price change percent
        /// </summary>
        public decimal PriceChangePercent { get; }

        /// <summary>
        /// Get the weighted average price.
        /// </summary>
        public decimal WeightedAveragePrice { get; }

        /// <summary>
        /// Get the previous close price.
        /// </summary>
        public decimal PreviousClosePrice { get; }

        /// <summary>
        /// Get the last price.
        /// </summary>
        public decimal LastPrice { get; }

        /// <summary>
        /// Get the last quantity.
        /// </summary>
        public decimal LastQuantity { get; }

        /// <summary>
        /// Get the bid price.
        /// </summary>
        public decimal BidPrice { get; }

        /// <summary>
        /// Get the bid quantity.
        /// </summary>
        public decimal BidQuantity { get; }

        /// <summary>
        /// Get the ask price.
        /// </summary>
        public decimal AskPrice { get; }

        /// <summary>
        /// Get the ask quantity.
        /// </summary>
        public decimal AskQuantity { get; }

        /// <summary>
        /// Get the open price.
        /// </summary>
        public decimal OpenPrice { get; }

        /// <summary>
        /// Get the high price.
        /// </summary>
        public decimal HighPrice { get; }

        /// <summary>
        /// Get the low price.
        /// </summary>
        public decimal LowPrice { get; }

        /// <summary>
        /// Get the base asset volume.
        /// </summary>
        public decimal Volume { get; }

        /// <summary>
        /// Get the quote asset volume.
        /// </summary>
        public decimal QuoteVolume { get; }

        /// <summary>
        /// Get the open time.
        /// </summary>
        public decimal OpenTime { get; }

        /// <summary>
        /// Get the close time.
        /// </summary>
        public decimal CloseTime { get; }

        /// <summary>
        /// Get the first trade ID.
        /// </summary>
        public long FirstTradeId { get; }

        /// <summary>
        /// Get the last trade ID.
        /// </summary>
        public long LastTradeId { get; }

        /// <summary>
        /// Get the trade count.
        /// </summary>
        public long TradeCount { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="period"></param>
        /// <param name="priceChange"></param>
        /// <param name="priceChangePercent"></param>
        /// <param name="weightedAveragePrice"></param>
        /// <param name="previousClosePrice"></param>
        /// <param name="lastPrice"></param>
        /// <param name="lastQuantity"></param>
        /// <param name="bidPrice"></param>
        /// <param name="bidQuantity"></param>
        /// <param name="askPrice"></param>
        /// <param name="askQuantity"></param>
        /// <param name="openPrice"></param>
        /// <param name="highPrice"></param>
        /// <param name="lowPrice"></param>
        /// <param name="volume"></param>
        /// <param name="quoteVolume"></param>
        /// <param name="openTime"></param>
        /// <param name="closeTime"></param>
        /// <param name="firstTradeId"></param>
        /// <param name="lastTradeId"></param>
        /// <param name="tradeCount"></param>
        public SymbolStatistics(
            string symbol,
            TimeSpan period,
            decimal priceChange,
            decimal priceChangePercent,
            decimal weightedAveragePrice,
            decimal previousClosePrice,
            decimal lastPrice,
            decimal lastQuantity,
            decimal bidPrice,
            decimal bidQuantity,
            decimal askPrice,
            decimal askQuantity,
            decimal openPrice,
            decimal highPrice,
            decimal lowPrice,
            decimal volume,
            decimal quoteVolume,
            long openTime,
            long closeTime,
            long firstTradeId,
            long lastTradeId,
            long tradeCount)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (weightedAveragePrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(weightedAveragePrice));
            if (previousClosePrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(previousClosePrice));
            if (lastPrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(lastPrice));
            if (lastQuantity < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: quantity must not be less than 0.", nameof(lastQuantity));
            if (bidPrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(bidPrice));
            if (bidQuantity < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: quantity must not be less than 0.", nameof(bidQuantity));
            if (askPrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(askPrice));
            if (askQuantity < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: quantity must not be less than 0.", nameof(askQuantity));
            if (openPrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(openPrice));
            if (highPrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(highPrice));
            if (lowPrice < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: price must not be less than 0.", nameof(lowPrice));
            if (lowPrice > highPrice)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: low price must be less than or equal to high price.", nameof(lowPrice));

            if (volume < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: volume must be greater than or equal to 0.", nameof(volume));
            if (quoteVolume < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: volume must be greater than or equal to 0.", nameof(quoteVolume));

            if (openTime <= 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: time must be greater than 0.", nameof(openTime));
            if (closeTime <= 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: time must be greater than 0.", nameof(closeTime));
            if (openTime >= closeTime)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: open time must be less than close time.", nameof(openTime));

            //if (firstTradeId < 0) // ...fails due to symbol 'ETC' (?) with -1 trade ID.
            //    throw new ArgumentException($"{nameof(Symbol24hrStats)} trade ID must be greater than 0.", nameof(firstTradeId));
            //if (lastTradeId < 0) // ...fails due to symbol 'ETC' (?) with -1 trade ID.
            //    throw new ArgumentException($"{nameof(Symbol24hrStats)} trade ID must be greater than 0.", nameof(lastTradeId));
            if (lastTradeId < firstTradeId)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: last trade ID must be greater than or equal to first trade ID.", nameof(lastTradeId));

            if (tradeCount < 0)
                throw new ArgumentException($"{nameof(SymbolStatistics)}: trade count must be greater than or equal to 0.", nameof(tradeCount));

            // TODO: Binance API stream occasionally returns invalid trade counts...
            //if (tradeCount != 0 && tradeCount != lastTradeId - firstTradeId + 1)
            //    throw new ArgumentException($"{nameof(SymbolStatistics)}: trade count must be equal to last trade ID - first trade ID + 1.", nameof(tradeCount));

            Symbol = symbol;
            Period = period;
            PriceChange = priceChange;
            PriceChangePercent = priceChangePercent;
            WeightedAveragePrice = weightedAveragePrice;
            PreviousClosePrice = previousClosePrice;
            LastPrice = lastPrice;
            LastQuantity = lastQuantity;
            BidPrice = bidPrice;
            BidQuantity = bidQuantity;
            AskPrice = askPrice;
            AskQuantity = askQuantity;
            OpenPrice = openPrice;
            HighPrice = highPrice;
            LowPrice = lowPrice;
            Volume = volume;
            QuoteVolume = quoteVolume;
            OpenTime = openTime;
            CloseTime = closeTime;
            FirstTradeId = firstTradeId;
            LastTradeId = lastTradeId;
            TradeCount = tradeCount;
        }

        #endregion Constructors
    }
}
