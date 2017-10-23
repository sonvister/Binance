using System;
using Xunit;

namespace Binance.Tests
{
    public class SymbolPriceTest
    {
        [Fact]
        public void Throws()
        {
            string symbol = Symbol.BTC_USDT;
            decimal value = 1.2345m;

            Assert.Throws<ArgumentNullException>("symbol", () => new SymbolPrice(null, value));
            Assert.Throws<ArgumentException>("value", () => new SymbolPrice(symbol, -1));
        }

        [Fact]
        public void Properties()
        {
            string symbol = Symbol.BTC_USDT;
            decimal value = 1.2345m;

            var price = new SymbolPrice(symbol, value);

            Assert.Equal(symbol, price.Symbol);
            Assert.Equal(value, price.Value);
        }
    }
}
