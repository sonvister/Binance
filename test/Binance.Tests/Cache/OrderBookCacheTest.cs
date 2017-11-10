using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Moq;
using Xunit;

namespace Binance.Tests.Cache
{
    public class OrderBookCacheTest
    {
        [Fact]
        public async Task SubscribeThrows()
        {
            var api = new Mock<IBinanceApi>().Object;
            var client = new Mock<IDepthWebSocketClient>().Object;

            var cache = new OrderBookCache(api, client);

            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => cache.SubscribeAsync(null, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>("token", () => cache.SubscribeAsync(Symbol.BTC_USDT, CancellationToken.None));
        }
    }
}
