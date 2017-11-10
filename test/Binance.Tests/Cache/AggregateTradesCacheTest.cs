using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Moq;
using System;
using Xunit;
using System.Threading.Tasks;
using System.Threading;

namespace Binance.Tests.Cache
{
    public class AggregateTradesCacheTest
    {
        [Fact]
        public async Task SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<ITradesWebSocketClient>().Object;

            var cache = new AggregateTradesCache(api, client);

            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>("token", () => cache.SubscribeAsync(Symbol.BTC_USDT, CancellationToken.None));
        }
    }
}
