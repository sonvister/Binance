using System;
using Binance.Api;
using Binance.Cache;
using Binance.Market;
using Binance.WebSocket;
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
            var client = new Mock<ICandlestickWebSocketClient>().Object;

            var cache = new CandlestickCache(api, client);

            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(null, CandlestickInterval.Day));
            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(string.Empty, CandlestickInterval.Day));

            cache.Subscribe(symbol, interval);

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe(symbol, interval));
        }
    }
}
