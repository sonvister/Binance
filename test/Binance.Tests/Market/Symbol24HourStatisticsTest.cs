using System;
using Binance.Market;
using Xunit;

namespace Binance.Tests.Market
{
    public class Symbol24HourStatisticsTest
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal priceChange = 50;
            const decimal priceChangePercent = 1;
            const decimal weightedAveragePrice = 5001;
            const decimal previousClosePrice = 4900;
            const decimal lastPrice = 5000;
            const decimal bidPrice = 4995;
            const decimal askPrice = 5005;
            const decimal openPrice = 4950;
            const decimal highPrice = 5025;
            const decimal lowPrice = 4925;
            const decimal volume = 100000;
            var openTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var closeTime = DateTimeOffset.FromUnixTimeMilliseconds(openTime).AddHours(24).ToUnixTimeMilliseconds();
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const long tradeCount = lastTradeId - firstTradeId + 1;

            Assert.Throws<ArgumentNullException>("symbol", () => new Symbol24HourStatistics(null, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("weightedAveragePrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, -1, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("previousClosePrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, -1, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("lastPrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, -1, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("bidPrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, -1, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("askPrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, -1, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("openPrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, -1, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("highPrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, -1, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("lowPrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, -1, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("lowPrice", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, lowPrice, highPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("volume", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, -1, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("openTime", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, -1, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("openTime", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, 0, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("closeTime", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, -1, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("closeTime", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, 0, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("openTime", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, closeTime, openTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("volume", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, -1, openTime, closeTime, lastTradeId, firstTradeId, tradeCount));

            Assert.Throws<ArgumentException>("lastTradeId", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, lastTradeId, firstTradeId, tradeCount));

            Assert.Throws<ArgumentException>("tradeCount", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, -1));
            Assert.Throws<ArgumentException>("tradeCount", () => new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount + 1));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal priceChange = 50;
            const decimal priceChangePercent = 1;
            const decimal weightedAveragePrice = 5001;
            const decimal previousClosePrice = 4900;
            const decimal lastPrice = 5000;
            const decimal bidPrice = 4995;
            const decimal askPrice = 5005;
            const decimal openPrice = 4950;
            const decimal highPrice = 5025;
            const decimal lowPrice = 4925;
            const decimal volume = 100000;
            var openTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var closeTime = DateTimeOffset.FromUnixTimeMilliseconds(openTime).AddHours(24).ToUnixTimeMilliseconds();
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const long tradeCount = lastTradeId - firstTradeId + 1;

            var stats = new Symbol24HourStatistics(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount);

            Assert.Equal(symbol, stats.Symbol);
            Assert.Equal(priceChange, stats.PriceChange);
            Assert.Equal(priceChangePercent, stats.PriceChangePercent);
            Assert.Equal(weightedAveragePrice, stats.WeightedAveragePrice);
            Assert.Equal(previousClosePrice, stats.PreviousClosePrice);
            Assert.Equal(lastPrice, stats.LastPrice);
            Assert.Equal(bidPrice, stats.BidPrice);
            Assert.Equal(askPrice, stats.AskPrice);
            Assert.Equal(openPrice, stats.OpenPrice);
            Assert.Equal(highPrice, stats.HighPrice);
            Assert.Equal(lowPrice, stats.LowPrice);
            Assert.Equal(volume, stats.Volume);
            Assert.Equal(openTime, stats.OpenTime);
            Assert.Equal(closeTime, stats.CloseTime);
            Assert.Equal(firstTradeId, stats.FirstTradeId);
            Assert.Equal(lastTradeId, stats.LastTradeId);
            Assert.Equal(tradeCount, stats.TradeCount);
        }
    }
}
