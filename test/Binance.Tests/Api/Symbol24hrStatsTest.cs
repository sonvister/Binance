using System;
using Xunit;

namespace Binance.Api.Tests
{
    public class Symbol24hrStatsTest
    {
        [Fact]
        public void Throws()
        {
            string symbol = Symbol.BTC_USDT;
            decimal priceChange = 50;
            decimal priceChangePercent = 1;
            decimal weightedAveragePrice = 5001;
            decimal previousClosePrice = 4900;
            decimal lastPrice = 5000;
            decimal bidPrice = 4995;
            decimal askPrice = 5005;
            decimal openPrice = 4950;
            decimal highPrice = 5025;
            decimal lowPrice = 4925;
            decimal volume = 100000;
            long openTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long closeTime = DateTimeOffset.FromUnixTimeMilliseconds(openTime).AddHours(24).ToUnixTimeMilliseconds();
            long firstTradeId = 123456;
            long lastTradeId = 234567;
            long tradeCount = lastTradeId - firstTradeId + 1;

            Assert.Throws<ArgumentNullException>("symbol", () => new Symbol24hrStats(null, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("weightedAveragePrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, -1, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("previousClosePrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, -1, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("lastPrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, -1, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("bidPrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, -1, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("askPrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, -1, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("openPrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, -1, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("highPrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, -1, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("lowPrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, -1, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("lowPrice", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, lowPrice, highPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("volume", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, -1, openTime, closeTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("openTime", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, -1, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("openTime", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, 0, closeTime, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("closeTime", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, -1, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("closeTime", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, 0, firstTradeId, lastTradeId, tradeCount));
            Assert.Throws<ArgumentException>("openTime", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, closeTime, openTime, firstTradeId, lastTradeId, tradeCount));

            Assert.Throws<ArgumentException>("volume", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, -1, openTime, closeTime, lastTradeId, firstTradeId, tradeCount));

            Assert.Throws<ArgumentException>("lastTradeId", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, lastTradeId, firstTradeId, tradeCount));

            Assert.Throws<ArgumentException>("tradeCount", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, -1));
            Assert.Throws<ArgumentException>("tradeCount", () => new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount + 1));
        }

        [Fact]
        public void Properties()
        {
            string symbol = Symbol.BTC_USDT;
            decimal priceChange = 50;
            decimal priceChangePercent = 1;
            decimal weightedAveragePrice = 5001;
            decimal previousClosePrice = 4900;
            decimal lastPrice = 5000;
            decimal bidPrice = 4995;
            decimal askPrice = 5005;
            decimal openPrice = 4950;
            decimal highPrice = 5025;
            decimal lowPrice = 4925;
            decimal volume = 100000;
            long openTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long closeTime = DateTimeOffset.FromUnixTimeMilliseconds(openTime).AddHours(24).ToUnixTimeMilliseconds();
            long firstTradeId = 123456;
            long lastTradeId = 234567;
            long tradeCount = lastTradeId - firstTradeId + 1;

            var stats = new Symbol24hrStats(symbol, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, bidPrice, askPrice, openPrice, highPrice, lowPrice, volume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount);

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
