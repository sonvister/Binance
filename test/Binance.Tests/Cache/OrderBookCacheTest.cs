using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Moq;
using System;
using Xunit;
using System.Threading.Tasks;

namespace Binance.Tests.Cache
{
    public class OrderBookCacheTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IDepthWebSocketClient>().Object;

            var cache = new OrderBookCache(api, client);

            return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null));
        }
    }
}
