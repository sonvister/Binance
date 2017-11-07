using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Moq;
using System;
using Xunit;

namespace Binance.Tests.Cache
{
    public class AggregateTradesCacheTest
    {
        [Fact]
        public void SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ITradesWebSocketClient>().Object;

            var cache = new AggregateTradesCache(api, client);

            Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null));
        }
    }
}
