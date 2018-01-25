using System;

namespace Binance.Market
{
    /// <summary>
    /// Candlestick/K-Line which is uniquely identified by the symbol,
    /// interval, and open time.
    /// </summary>
    public sealed class Candlestick : IEquatable<Candlestick>, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the interval.
        /// </summary>
        public CandlestickInterval Interval { get; }

        /// <summary>
        /// Get the open time.
        /// </summary>
        public long OpenTime { get; }

        /// <summary>
        /// Get the open price in quote asset units.
        /// </summary>
        public decimal Open { get; }

        /// <summary>
        /// Get the high price in quote asset units.
        /// </summary>
        public decimal High { get; }

        /// <summary>
        /// Get the low price in quote asset units.
        /// </summary>
        public decimal Low { get; }

        /// <summary>
        /// Get the close price in quote asset units.
        /// </summary>
        public decimal Close { get; }

        /// <summary>
        /// Get the volume in base asset units.
        /// </summary>
        public decimal Volume { get; }

        /// <summary>
        /// Get the close time.
        /// </summary>
        public long CloseTime { get; }

        /// <summary>
        /// Get the volume in quote asset units.
        /// </summary>
        public decimal QuoteAssetVolume { get; }

        /// <summary>
        /// Get the number of trades.
        /// </summary>
        public long NumberOfTrades { get; }

        /// <summary>
        /// Get the taker buy base asset volume.
        /// </summary>
        public decimal TakerBuyBaseAssetVolume { get; }

        /// <summary>
        /// Get the taker buy quote asset volume.
        /// </summary>
        public decimal TakerBuyQuoteAssetVolume { get; }

        /// <summary>
        /// Get the candlestick timestamp.
        /// </summary>
        public long Timestamp => OpenTime;

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="openTime">The open time.</param>
        /// <param name="open">The open price.</param>
        /// <param name="high">The high price.</param>
        /// <param name="low">The low price.</param>
        /// <param name="close">The close price.</param>
        /// <param name="volume">The volume in base asset units.</param>
        /// <param name="closeTime">The close time.</param>
        /// <param name="quoteAssetVolume">The volume in quote assset units.</param>
        /// <param name="numberOfTrades">The number of trades.</param>
        /// <param name="takerBuyBaseAssetVolume">The taker buy base asset volume.</param>
        /// <param name="takerBuyQuoteAssetVolume">The taker buy quote asset volume.</param>
        public Candlestick(
            string symbol,
            CandlestickInterval interval,
            long openTime,
            decimal open,
            decimal high,
            decimal low,
            decimal close,
            decimal volume,
            long closeTime,
            decimal quoteAssetVolume,
            long numberOfTrades,
            decimal takerBuyBaseAssetVolume,
            decimal takerBuyQuoteAssetVolume)
        {
            Throw.IfNull(symbol, nameof(symbol));

            if (openTime <= 0)
                throw new ArgumentException($"{nameof(Candlestick)}: timestamp must be greater than 0.", nameof(openTime));
            if (closeTime <= 0)
                throw new ArgumentException($"{nameof(Candlestick)}: timestamp must be greater than 0.", nameof(closeTime));

            if (open < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: price must not be less than 0.", nameof(open));
            if (high < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: price must not be less than 0.", nameof(high));
            if (low < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: price must not be less than 0.", nameof(low));
            if (close < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: price must not be less than 0.", nameof(close));

            if (numberOfTrades < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: number of trades must not be less than 0.", nameof(numberOfTrades));

            if (volume < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: volume must not be less than 0.", nameof(volume));
            if (quoteAssetVolume < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: volume must not be less than 0.", nameof(quoteAssetVolume));
            if (takerBuyBaseAssetVolume < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: volume must not be less than 0.", nameof(takerBuyBaseAssetVolume));
            if (takerBuyQuoteAssetVolume < 0)
                throw new ArgumentException($"{nameof(Candlestick)}: volume must not be less than 0.", nameof(takerBuyQuoteAssetVolume));

            Symbol = symbol.FormatSymbol();
            Interval = interval;
            OpenTime = openTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            CloseTime = closeTime;
            QuoteAssetVolume = quoteAssetVolume;
            NumberOfTrades = numberOfTrades;
            TakerBuyBaseAssetVolume = takerBuyBaseAssetVolume;
            TakerBuyQuoteAssetVolume = takerBuyQuoteAssetVolume;
        }

        #endregion Constructors

        #region IEquatable

        public bool Equals(Candlestick other)
        {
            if (other == null)
                return false;

            return other.Symbol == Symbol
                && other.Interval == Interval
                && other.OpenTime == OpenTime
                && other.Open == Open
                && other.High == High
                && other.Low == Low
                && other.Close == Close
                && other.Volume == Volume
                && other.CloseTime == CloseTime
                && other.QuoteAssetVolume == QuoteAssetVolume
                && other.NumberOfTrades == NumberOfTrades
                && other.TakerBuyBaseAssetVolume == TakerBuyBaseAssetVolume
                && other.TakerBuyQuoteAssetVolume == TakerBuyQuoteAssetVolume;
        }

        #endregion IEquatable
    }
}
