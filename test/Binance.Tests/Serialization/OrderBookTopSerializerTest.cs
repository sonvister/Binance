using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class OrderBookTopSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal bidPrice = 0.123456789m;
            const decimal bidQuantity = 0.987654321m;
            const decimal askPrice = 1.123456789m;
            const decimal askQuantity = 1.987654321m;

            var top = OrderBookTop.Create(symbol, bidPrice, bidQuantity, askPrice, askQuantity);

            var serializer = new OrderBookTopSerializer();

            var json = serializer.Serialize(top);

            var other = serializer.Deserialize(json);

            Assert.True(top.Equals(other));
        }
    }
}
