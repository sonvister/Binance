using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class OrderBookSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            var serializer = new OrderBookSerializer();

            var json = serializer.Serialize(orderBook);

            var other = serializer.Deserialize(json);

            Assert.True(orderBook.Equals(other));
        }
    }
}
