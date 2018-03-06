using System;
using Binance.Cache;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class AggregateTradeCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var symbol = Symbol.BTC_USDT;
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IAggregateTradeWebSocketClient>().Object;

            var cache = new AggregateTradeCache(api, client);

            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(null));
            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(string.Empty));

            cache.Subscribe(symbol);

            Assert.Throws<InvalidOperationException>(() => cache.Subscribe(symbol));
        }
    }
}
