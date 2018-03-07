using System;
using Binance.Cache;
using Binance.Client;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class CandlestickCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var symbol = Symbol.BTC_USDT;
            var interval = CandlestickInterval.Day;
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ICandlestickClient>().Object;

            var cache = new CandlestickCache(api, client);

            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(null, CandlestickInterval.Day));
            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(string.Empty, CandlestickInterval.Day));

            cache.Subscribe(symbol, interval);

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe(symbol, interval));
        }

        [Fact]
        public void Unsubscribe()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ICandlestickClient>().Object;

            var cache = new CandlestickCache(api, client);

            // Can call unsubscribe before subscribe or multiple times without fail.
            cache.Unsubscribe();
            cache.Unsubscribe();
        }
    }
}
