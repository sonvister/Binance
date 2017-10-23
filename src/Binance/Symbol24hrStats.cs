using System;

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

            if (weightedAveragePrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(weightedAveragePrice));
            if (previousClosePrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(previousClosePrice));
            if (lastPrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(lastPrice));
            if (bidPrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(bidPrice));
            if (askPrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(askPrice));
            if (openPrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(openPrice));
            if (highPrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(highPrice));
            if (lowPrice < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} price must not be less than 0.", nameof(lowPrice));
            if (lowPrice > highPrice)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} low price must be less than or equal to high price.", nameof(lowPrice));

            if (volume < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} volume must be greater than or equal to 0.", nameof(volume));

            if (openTime <= 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} time must be greater than 0.", nameof(openTime));
            if (closeTime <= 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} time must be greater than 0.", nameof(closeTime));
            if (openTime >= closeTime)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} open time must be less than close time.", nameof(openTime));

            //if (firstTradeId < 0) // ...fails due to symbol 'ETC' (?) with -1 trade ID.
            //    throw new ArgumentException($"{nameof(Symbol24hrStats)} trade ID must be greater than 0.", nameof(firstTradeId));
            //if (lastTradeId < 0) // ...fails due to symbol 'ETC' (?) with -1 trade ID.
            //    throw new ArgumentException($"{nameof(Symbol24hrStats)} trade ID must be greater than 0.", nameof(lastTradeId));
            if (lastTradeId < firstTradeId)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} last trade ID must be greater than or equal to first trade ID.", nameof(lastTradeId));

            if (tradeCount < 0)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} trade count must be greater than or equal to 0.", nameof(tradeCount));
            if (tradeCount != 0 && tradeCount != lastTradeId - firstTradeId + 1)
                throw new ArgumentException($"{nameof(Symbol24hrStats)} trade count must be equal to last trade ID - first trade ID + 1.", nameof(tradeCount));

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
