using System;
using Binance.Api;
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
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IAggregateTradeWebSocketClient>().Object;

            var cache = new AggregateTradeCache(api, client);

            Assert.Throws<ArgumentNullException>("symbol", () => cache.Subscribe(null));
        }
    }
}
