using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class SymbolPriceSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal value = 1.2345m;

            var price = new SymbolPrice(symbol, value);

            var serializer = new SymbolPriceSerializer();

            var json = serializer.Serialize(price);

            var other = serializer.Deserialize(json);

            Assert.True(price.Equals(other));
        }
    }
}
