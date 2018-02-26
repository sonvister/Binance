using System;
using Binance.Api;
using Binance.Cache;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class TradeCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var symbol = Symbol.BTC_USDT;
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ITradeWebSocketClient>().Object;

            var cache = new TradeCache(api, client);

            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(null));
            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(string.Empty));

            cache.Subscribe(symbol);

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe(symbol));
        }
    }
}
