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
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ITradeWebSocketClient>().Object;

            var cache = new TradeCache(api, client);

            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(null));
        }
    }
}
