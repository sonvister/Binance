using System;
using Binance.Market;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Market
{
    public class SymbolPriceTest
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal value = 1.2345m;

            Assert.Throws<ArgumentNullException>("symbol", () => new SymbolPrice(null, value));
            Assert.Throws<ArgumentException>("value", () => new SymbolPrice(symbol, -1));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal value = 1.2345m;

            var price = new SymbolPrice(symbol, value);

            Assert.Equal(symbol, price.Symbol);
            Assert.Equal(value, price.Value);
        }

        [Fact]
        public void Serialization()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal value = 1.2345m;

            var price = new SymbolPrice(symbol, value);

            var json = JsonConvert.SerializeObject(price);

            price = JsonConvert.DeserializeObject<SymbolPrice>(json);

            Assert.Equal(symbol, price.Symbol);
            Assert.Equal(value, price.Value);
        }
    }
}
