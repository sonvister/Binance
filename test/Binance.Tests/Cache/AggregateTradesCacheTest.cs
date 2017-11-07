using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Moq;
using System;
using Xunit;
using System.Threading.Tasks;

namespace Binance.Tests.Cache
{
    public class AggregateTradesCacheTest
    {
        [Fact]
        public Task SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ITradesWebSocketClient>().Object;

            var cache = new AggregateTradesCache(api, client);

            return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null));
        }
    }
}
