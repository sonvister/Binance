using System;
using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class SymbolStatisticsSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            var period = TimeSpan.FromHours(24);
            const decimal priceChange = 50;
            const decimal priceChangePercent = 1;
            const decimal weightedAveragePrice = 5001;
            const decimal previousClosePrice = 4900;
            const decimal lastPrice = 5000;
            const decimal lastQuantity = 1;
            const decimal bidPrice = 4995;
            const decimal bidQuantity = 2;
            const decimal askPrice = 5005;
            const decimal askQuantity = 3;
            const decimal openPrice = 4950;
            const decimal highPrice = 5025;
            const decimal lowPrice = 4925;
            const decimal volume = 100000;
            const decimal quoteVolume = 200000;
            var openTime = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            var closeTime = openTime.AddHours(24);
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const long tradeCount = lastTradeId - firstTradeId + 1;

            var stats = new SymbolStatistics(symbol, period, priceChange, priceChangePercent, weightedAveragePrice, previousClosePrice, lastPrice, lastQuantity, bidPrice, bidQuantity, askPrice, askQuantity, openPrice, highPrice, lowPrice, volume, quoteVolume, openTime, closeTime, firstTradeId, lastTradeId, tradeCount);

            var serializer = new SymbolStatisticsSerializer();

            var json = serializer.Serialize(stats);

            var other = serializer.Deserialize(json);

            Assert.True(stats.Equals(other));
        }
    }
}
