using System;
using Binance.Market;
using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class TradeSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const long buyerOrderId = 123456;
            const long sellerOrderId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new Trade(symbol, id, price, quantity, buyerOrderId, sellerOrderId, time, isBuyerMaker, isBestPriceMatch);

            var serializer = new TradeSerializer();

            var json = serializer.Serialize(trade);

            var other = serializer.Deserialize(json);

            Assert.True(trade.Equals(other));
        }
    }
}
