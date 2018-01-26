using System;
using Binance.Market;
using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class AggregateTradeSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, time, isBuyerMaker, isBestPriceMatch);

            var serializer = new AggregateTradeSerializer();

            var json = serializer.Serialize(trade);

            var other = serializer.Deserialize(json);

            Assert.True(trade.Equals(other));
        }
    }
}
